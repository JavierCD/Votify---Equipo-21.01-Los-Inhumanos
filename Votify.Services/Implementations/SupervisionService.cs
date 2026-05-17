using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Interfaces;
using Votify.Services.Models.Responses;

namespace Votify.Services.Implementations
{
    public class SupervisionService : ISupervisionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SupervisionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<JuezSupervisionDto>> ObtenerEstadoJuecesAsync(int votacionId)
        {
            // Paso 1: Obtener la votación y su categoría para saber el eventoId
            var votacion = await _unitOfWork.Votaciones.GetWithIncludesAsync(
                v => v.Id == votacionId,
                v => v.Categoria
            );

            if (votacion?.Categoria == null)
                throw new KeyNotFoundException($"Votación no encontrada.");

            var eventoId = votacion.Categoria.EventoId;

            // Paso 2: Obtener el evento con sus jueces
            var evento = await _unitOfWork.Eventos.GetWithIncludesAsync(
                e => e.Id == eventoId,
                e => e.Jurado
            );

            if (evento == null)
                throw new KeyNotFoundException($"Evento no encontrado.");

            var jueces = evento.Jurado?.ToList() ?? new List<Juez>();

            // Paso 3: Obtener los IDs de jueces que ya han votado
            var juecesQueHanVotado = await _unitOfWork.VotoExpertoRepository.ObtenerJuecesQueHanVotadoAsync(votacionId);

            // Paso 4: Construir el resultado
            var resultado = new List<JuezSupervisionDto>();
            foreach (var juez in jueces)
            {
                resultado.Add(new JuezSupervisionDto
                {
                    JuezId = juez.Id,
                    Nombre = juez.Name,
                    Email = juez.Email,
                    HaVotado = juecesQueHanVotado.Contains(juez.Id)
                });
            }

            return resultado;
        }

        public async Task EnviarRecordatorioAsync(int juezId, int votacionId, int categoriaId, string categoriaNombre, string eventoNombre)
        {
            var juez = await _unitOfWork.Miembros.GetByIdAsync(juezId) as Juez;
            if (juez == null)
                throw new KeyNotFoundException($"Juez con ID {juezId} no encontrado.");

            var votacion = await _unitOfWork.Votaciones.GetWithIncludesAsync(
                v => v.Id == votacionId,
                v => v.Categoria,
                v => v.Categoria.Evento
            );

            var eventoId = votacion?.Categoria?.Evento?.Id ?? 0;

            var notificacion = new Notificacion(
                miembroId: juezId,
                titulo: "⏳ Recordatorio de evaluación pendiente",
                mensaje: $"Aún no has emitido tu evaluación para la categoría '{categoriaNombre}' del evento '{eventoNombre}'. Por favor, completa tu voto antes del cierre.",
                urlAccion: $"/juez/evento/{eventoId}"
            );

            await _unitOfWork.Notificaciones.AddAsync(notificacion);
            await _unitOfWork.SaveChangesAsync();
        }
       
    }
}