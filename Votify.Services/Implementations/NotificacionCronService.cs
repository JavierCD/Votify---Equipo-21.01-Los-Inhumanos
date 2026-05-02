using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Interfaces;

namespace Votify.Services.Implementations
{
    public class NotificacionCronService : INotificacionCronService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificacionCronService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ProcesarAperturasDeVotacionAsync()
        {
            var ahora = DateTime.UtcNow;

            var todasVotaciones = await _unitOfWork.Votaciones.GetAllWithIncludesAsync(
                v => v.Categoria,
                v => v.Categoria.Evento,
                v => v.Categoria.Evento.Jurado
            );

            var votacionesPendientes = todasVotaciones
                .Where(v => v.FechaApertura <= ahora
                         && v.EnviarNotificacionApertura == true
                         && v.NotificacionAperturaEnviada == false)
                .ToList();

            if (!votacionesPendientes.Any()) return;

            foreach (var votacion in votacionesPendientes)
            {
                var evento = votacion.Categoria?.Evento;
                if (evento == null || evento.Jurado == null) continue;

                var juecesAvisables = evento.Jurado.Where(j => j.QuiereRecibirNotificaciones).ToList();

                foreach (var juez in juecesAvisables)
                {
                    var notificacion = new Notificacion(
                        miembroId: juez.Id,
                        titulo: "¡Votación Abierta!",
                        mensaje: $"La evaluación para la categoría '{votacion.Categoria!.Name}' del evento '{evento.Name}' ha comenzado. ¡Ya puedes emitir tus votos!",
                        urlAccion: $"/voto-popular-usuario/{votacion.Id}"
                    );

                    await _unitOfWork.Notificaciones.AddAsync(notificacion);
                }

                votacion.NotificacionAperturaEnviada = true;
                votacion.Estado = "Abierta";
                await _unitOfWork.Votaciones.UpdateAsync(votacion);
            }

            await _unitOfWork.SaveChangesAsync();
        }
       
    }
}