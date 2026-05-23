using System.Security.Cryptography;
using System.Text;
using Votify.Core.Factories;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Interfaces;
using Votify.Services.Models.Requests;
using Votify.Services.Models.Responses;

namespace Votify.Services.Implementations
{
    public class VotoPuntuacionService : IVotoPuntuacionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVotoValidationHandler _validationChain;

        public VotoPuntuacionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _validationChain = VotoValidationChainBuilder.BuildPuntuacionChain(
                obtenerVotacion: id => _unitOfWork.VotoPuntuacionRepository.ObtenerVotacionPuntuacionPorIdAsync(id),
                obtenerProyectos: catId => _unitOfWork.VotoPuntuacionRepository.ObtenerProyectosPorCategoriaAsync(catId),
                emailYaVoto: (votId, email) => _unitOfWork.VotoPuntuacionRepository.EmailYaVotoEnVotacionAsync(votId, email)
            );
        }

        public async Task<VotacionPuntuacionDetalleResponse> ObtenerDetallePorIdAsync(int votacionId)
        {
            var votacion = await _unitOfWork.VotoPuntuacionRepository.ObtenerVotacionPuntuacionPorIdAsync(votacionId);

            if (votacion == null)
                throw new InvalidOperationException("La votación no existe o no está disponible.");

            var proyectos = await _unitOfWork.VotoPuntuacionRepository.ObtenerProyectosPorCategoriaAsync(votacion.CategoriaId);

            return new VotacionPuntuacionDetalleResponse
            {
                VotacionId = votacion.Id,
                CategoriaId = votacion.CategoriaId,
                CategoriaNombre = votacion.Categoria?.Name ?? "Sin categoría",
                Estado = votacion.Estado,
                ValorMax = votacion.ValorMax,
                PermiteAutoVoto = votacion.PermiteAutoVoto,
                Proyectos = proyectos.Select(p => new ProyectoVotacionPopularResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    ParticipanteId = p.ParticipanteId
                }).ToList()
            };
        }

        public async Task EmitirVotoPuntuacionAsync(EmitirVotoPuntuacionRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var context = new VotoValidationContext
            {
                VotacionId = request.VotacionId,
                VotanteId = request.VotanteId,
                JuezId = request.JuezId,
                Email = request.Email,
                Anonimo = request.Anonimo,
                PuntuacionesPorProyecto = request.PuntuacionesPorProyecto
            };

            await _validationChain.HandleAsync(context);

            var votacion = (Puntuacion)context.Votacion!;

            Votante votanteFinal = null;
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var todosLosVotantes = await _unitOfWork.Votantes.GetAllAsync();
                votanteFinal = todosLosVotantes.FirstOrDefault(v => v.Email == request.Email);
                if (votanteFinal == null)
                {
                    votanteFinal = new Votante { Email = request.Email };
                    await _unitOfWork.Votantes.AddAsync(votanteFinal);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            bool esJuez = request.JuezId.HasValue && request.JuezId.Value > 0;
            VotoCreator creadorVoto = esJuez ? new VotoExpertoCreator() : new VotoPublicoCreator();
            List<Voto> votosAInsertar = new List<Voto>();

            foreach (var kvp in request.PuntuacionesPorProyecto)
            {
                int proyectoId = kvp.Key;
                double puntuacion = kvp.Value;

                Voto? papeleta = votacion.Votos?.FirstOrDefault(v =>
                    v.ProyectoId == proyectoId &&
                    (
                        (esJuez && v is VotoExperto ve && ve.JuezId == request.JuezId!.Value) ||
                        (!esJuez && v is VotoPublico vp && vp.VotanteId == (votanteFinal != null ? votanteFinal.Id : request.VotanteId))
                    )
                );

                if (papeleta != null)
                {
                    papeleta.PuntuacionBase = puntuacion;
                }
                else
                {
                    string? hash = null;
                    if (request.Anonimo)
                    {
                        hash = Convert.ToHexString(
                            SHA256.HashData(
                                Encoding.UTF8.GetBytes($"{request.VotacionId}-{proyectoId}-{DateTime.UtcNow.Ticks}")
                            )
                        ).Substring(0, 16);
                    }

                    papeleta = creadorVoto.CrearVoto(
                        request.VotacionId,
                        proyectoId,
                        puntuacion,
                        request.Anonimo,
                        hash
                    );

                    if (esJuez) papeleta.AsignarEmisorId(request.JuezId!.Value);
                    else if (votanteFinal != null) papeleta.AsignarEmisorId(votanteFinal.Id);
                    else papeleta.AsignarEmisorId(request.VotanteId);

                    votosAInsertar.Add(papeleta);
                }
            }

            if (votosAInsertar.Any())
                await _unitOfWork.VotoPuntuacionRepository.GuardarVotosAsync(votosAInsertar);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
