using System.Security.Cryptography;
using System.Text;
using Votify.Core.Factories;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Interfaces;
using Votify.Services.Models;

namespace Votify.Services.Implementations
{
    public class VotoMulticriterioService : IVotoMulticriterioService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VotoMulticriterioService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<VotacionMulticriterioDetalleResponse>> ObtenerVotacionesMulticriterioDisponiblesAsync()
        {
            var votaciones = await _unitOfWork.VotoMulticriterioRepository.ObtenerVotacionesMulticriterioDisponiblesAsync();

            return votaciones.Select(v => new VotacionMulticriterioDetalleResponse
            {
                VotacionId = v.Id,
                CategoriaNombre = v.Categoria?.Name ?? "Sin Categoría",
                Estado = v.Estado
            }).ToList();
        }

        public async Task<List<VotacionMulticriterioDetalleResponse>> ObtenerVotacionesDisponiblesAsync(int votanteId)
        {
            return await ObtenerVotacionesMulticriterioDisponiblesAsync();
        }

        public async Task<VotacionMulticriterioDetalleResponse> ObtenerDetallePorIdAsync(int votacionId)
        {
            var votacion = await _unitOfWork.VotoMulticriterioRepository.ObtenerVotacionMulticriterioPorIdAsync(votacionId);

            if (votacion == null)
                throw new Exception("La votación multicriterio no existe.");

            return new VotacionMulticriterioDetalleResponse
            {
                VotacionId = votacion.Id,
                CategoriaNombre = votacion.Categoria?.Name ?? "Categoría Sin Nombre",
                Estado = votacion.Estado,
                Proyectos = votacion.Categoria?.Proyectos?
                    .Where(p => p.Visible)
                    .Select(p => new VotacionMulticriterioDetalleResponse.ProyectoDto
                    {
                        Id = p.Id,
                        Name = p.Name
                    }).ToList() ?? new(),
                Criterios = votacion.Criterios?
                    .Select(c => new VotacionMulticriterioDetalleResponse.CriterioBaremoDto
                    {
                        Id = c.Id,
                        Nombre = c.Name,
                        Peso = c.Peso
                    }).ToList() ?? new()
            };
        }           

        public async Task EmitirVotoMulticriterioAsync(EmitirVotoMulticriterioRequest request)
        {
            var votacion = await _unitOfWork.VotoMulticriterioRepository.ObtenerVotacionMulticriterioPorIdAsync(request.VotacionId);

            if (votacion == null)
                throw new Exception("La votación especificada no existe.");

            if (!votacion.PuedeVotar(DateTime.UtcNow))
                throw new InvalidOperationException("La votación no está abierta en este momento.");

            var yaVoto = await _unitOfWork.VotoMulticriterioRepository.EmailYaVotoEnVotacionAsync(request.VotacionId, request.Email);
            if (yaVoto) throw new Exception("Este correo ya ha emitido una evaluación para esta categoría.");

            Votante? votanteFinal = null;

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var todosLosVotantes = await _unitOfWork.Votantes.GetAllAsync();
                votanteFinal = todosLosVotantes.FirstOrDefault(v => v.Email == request.Email);

                if (votanteFinal == null)
                {
                    votanteFinal = new Votante
                    {
                        Email = request.Email,
                    };

                    await _unitOfWork.Votantes.AddAsync(votanteFinal);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            bool esJuez = request.JuezId.HasValue && request.JuezId.Value > 0;

            VotoCreator creador = esJuez
                ? new VotoExpertoCreator()
                : new VotoPublicoCreator();
            List<Voto> votosAInsertar = new List<Voto>();

            foreach (var proyectoEvaluado in request.Puntuaciones)
            {
                int proyectoId = proyectoEvaluado.Key;
                double puntuacionBase = 0;

                
                Voto? papeleta = votacion.Votos?.FirstOrDefault(v =>
                    v.ProyectoId == proyectoId &&
                    (
                        (esJuez && v is VotoExperto ve && ve.JuezId == request.JuezId!.Value) ||
                        (!esJuez && v is VotoPublico vp && vp.VotanteId == (votanteFinal != null ? votanteFinal.Id : request.VotanteId))
                    )
                );

                bool esNuevo = false;
                if (papeleta == null)
                {
                    // 2. Si no hay comentario previo, creamos el voto de cero
                    string? hash = null;
                    if (request.Anonimo)
                    {
                        string identificadorSecreto = esJuez ? request.JuezId!.Value.ToString() : (!string.IsNullOrWhiteSpace(request.Email) ? request.Email : request.VotanteId.ToString());
                        hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{request.VotacionId}-{identificadorSecreto}-VotifySecretSalt2026"))).Substring(0, 16);
                    }

                    papeleta = creador.CrearVoto(request.VotacionId, proyectoId, puntuacionBase, request.Anonimo, hash);

                    if (esJuez) papeleta.AsignarEmisorId(request.JuezId!.Value);
                    else if (votanteFinal != null) papeleta.AsignarEmisorId(votanteFinal.Id);
                    else papeleta.AsignarEmisorId(request.VotanteId);

                    esNuevo = true;
                }

                // 3. Añadimos las puntuaciones a la papeleta (sea la nueva o la recuperada del comentario)
                foreach (var criterioEvaluado in proyectoEvaluado.Value)
                {
                    if (criterioEvaluado.Value > 0 && !papeleta.Detalles.Any(d => d.CriterioId == criterioEvaluado.Key))
                    {
                        papeleta.Detalles.Add(new DetalleVoto
                        {
                            ProyectoId = proyectoId,
                            CriterioId = criterioEvaluado.Key,
                            Puntuacion = criterioEvaluado.Value
                        });
                    }
                }

                if (esNuevo && papeleta.Detalles.Any())
                {
                    votosAInsertar.Add(papeleta); // Se hará INSERT
                }
                // Si no es nuevo, Entity Framework ya lo está rastreando (Tracking). 
                // Al hacer _unitOfWork.SaveChangesAsync() al final, hará un UPDATE automático.
            }

            if (votosAInsertar.Any())
            {
                await _unitOfWork.VotoMulticriterioRepository.GuardarVotosAsync(votosAInsertar);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}