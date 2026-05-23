using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Votify.Core.Factories;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Interfaces;
using Votify.Services.Models.Requests;
using Votify.Services.Models.Responses;

namespace Votify.Services.Implementations
{
    public class VotoPopularService : IVotoPopularService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVotoValidationHandler _validationChain;

        public VotoPopularService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _validationChain = VotoValidationChainBuilder.BuildPopularChain(
                obtenerVotacion: id => _unitOfWork.VotoPopularRepository.ObtenerVotacionPopularPorIdAsync(id),
                obtenerProyectos: catId => _unitOfWork.VotoPopularRepository.ObtenerProyectosPorCategoriaAsync(catId),
                emailYaVoto: (votId, email) => _unitOfWork.VotoPopularRepository.EmailYaVotoEnVotacionAsync(votId, email)
            );
        }

        public async Task<List<VotacionPopularDisponibleResponse>> ObtenerVotacionesPopularesDisponiblesAsync()
        {
            var votaciones = await _unitOfWork.VotoPopularRepository.ObtenerVotacionesPopularesDisponiblesAsync();

            return votaciones.Select(v => new VotacionPopularDisponibleResponse
            {
                VotacionId = v.Id,
                CategoriaId = v.CategoriaId,
                CategoriaNombre = v.Categoria?.Name ?? "Sin categoría",
                Estado = v.Estado,
                MaxSelection = v.MaxSelection,
                Proyectos = new()
            }).ToList();
        }

        public async Task<VotacionPopularDisponibleResponse> ObtenerDetallePorIdAsync(int votacionId)
        {
            var votacion = await _unitOfWork.VotoPopularRepository.ObtenerVotacionPopularPorIdAsync(votacionId);

            if (votacion == null)
                throw new InvalidOperationException("La votación no existe o no está disponible.");

            var proyectos = await _unitOfWork.VotoPopularRepository.ObtenerProyectosPorCategoriaAsync(votacion.CategoriaId);

            return new VotacionPopularDisponibleResponse
            {
                VotacionId = votacion.Id,
                CategoriaId = votacion.CategoriaId,
                CategoriaNombre = votacion.Categoria?.Name ?? "Sin categoría",
                Estado = votacion.Estado,
                MaxSelection = votacion.MaxSelection,
                Proyectos = proyectos.Select(p => new ProyectoVotacionPopularResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    ParticipanteId = p.ParticipanteId
                }).ToList()
            };
        }

        public async Task EmitirVotoPopularAsync(EmitirVotoPopularRequest request)
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
                ProyectosSeleccionadosIds = request.ProyectosSeleccionadosIds
            };

            await _validationChain.HandleAsync(context);

            var votacion = (Popular)context.Votacion!;

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
            double puntuacionBaseEmitida = 10.0;

            List<Voto> votosAInsertar = new List<Voto>();

            foreach (var proyectoId in request.ProyectosSeleccionadosIds)
            {
                Voto? papeleta = votacion.Votos?.FirstOrDefault(v =>
                    v.ProyectoId == proyectoId &&
                    (
                        (esJuez && v is VotoExperto ve && ve.JuezId == request.JuezId!.Value) ||
                        (!esJuez && v is VotoPublico vp && vp.VotanteId == (votanteFinal != null ? votanteFinal.Id : request.VotanteId))
                    )
                );

                if (papeleta != null)
                {
                    papeleta.PuntuacionBase = puntuacionBaseEmitida;
                }
                else
                {
                    string? hash = null;
                    if (request.Anonimo)
                    {
                        string identificadorSecreto = esJuez
                            ? request.JuezId!.Value.ToString()
                            : (!string.IsNullOrWhiteSpace(request.Email) ? request.Email : request.VotanteId.ToString());

                        hash = Convert.ToHexString(
                            SHA256.HashData(
                                Encoding.UTF8.GetBytes($"{request.VotacionId}-{identificadorSecreto}-VotifySecretSalt2026")
                            )
                        ).Substring(0, 16);
                    }

                    papeleta = creadorVoto.CrearVoto(
                        votacion.Id,
                        proyectoId,
                        puntuacionBaseEmitida,
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
                await _unitOfWork.VotoPopularRepository.GuardarVotosAsync(votosAInsertar);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<VotacionPopularDisponibleResponse>> ObtenerVotacionesDisponiblesAsync(int votanteId)
        {
            var votaciones = await _unitOfWork.VotoPopularRepository.ObtenerVotacionesPopularesDisponiblesAsync();

            return votaciones.Select(v => new VotacionPopularDisponibleResponse
            {
                VotacionId = v.Id,
                CategoriaId = v.CategoriaId,
                CategoriaNombre = v.Categoria?.Name ?? string.Empty,
                Estado = v.Estado,
                MaxSelection = v.MaxSelection,
                PermiteAutoVoto = v.PermiteAutoVoto,
                Proyectos = new List<ProyectoVotacionPopularResponse>()
            }).ToList();
        }
    }
}
