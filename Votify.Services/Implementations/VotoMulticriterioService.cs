using Microsoft.EntityFrameworkCore;
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

            VotoCreator creador = new VotoPublicoCreator();
            List<Voto> votosAInsertar = new List<Voto>();

            foreach (var proyectoEvaluado in request.Puntuaciones)
            {
                int proyectoId = proyectoEvaluado.Key;

                double puntuacionBase = 0;

                Voto papeleta = creador.CrearVoto(
                    votacionId: request.VotacionId,
                    proyectoId: proyectoId,
                    puntuacionBase: puntuacionBase,
                    anonimo: request.Anonimo
                );

                foreach (var criterioEvaluado in proyectoEvaluado.Value)
                {
                    if (criterioEvaluado.Value > 0)
                    {
                        papeleta.Detalles.Add(new DetalleVoto
                        {
                            ProyectoId = proyectoId,
                            CriterioId = criterioEvaluado.Key,
                            Puntuacion = criterioEvaluado.Value
                        });
                    }
                }

                if (papeleta.Detalles.Any())
                {
                    votosAInsertar.Add(papeleta);
                }
            }

            if (votosAInsertar.Any())
            {
                await _unitOfWork.VotoMulticriterioRepository.GuardarVotosAsync(votosAInsertar);
            }

            var registroVotante = new Votante
            {
                Email = request.Email
            };

            await _unitOfWork.Votantes.AddAsync(registroVotante);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}