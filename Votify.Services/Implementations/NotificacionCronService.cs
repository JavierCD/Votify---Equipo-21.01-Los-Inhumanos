using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Votify.Core.Models;
using Votify.Persistence.Context;
using Votify.Services.Interfaces;

namespace Votify.Services.Implementations
{
    public class NotificacionCronService : INotificacionCronService
    {
        private readonly VotifyContext _context;

        public NotificacionCronService(VotifyContext context)
        {
            _context = context;
        }

        public async Task ProcesarAperturasDeVotacionAsync()
        {
            var ahora = DateTime.UtcNow;

            // 1. Buscamos votaciones cuya fecha de inicio ya pasó, que el admin quiere notificar, 
            // y que AÚN NO han sido notificadas. Traemos sus categorías, evento y jurado.
            var votacionesPendientes = await _context.Votaciones
                .Include(v => v.Categoria)
                    .ThenInclude(c => c.Evento)
                        .ThenInclude(e => e.Jurado)
                .Where(v => v.FechaApertura <= ahora
                         && v.EnviarNotificacionApertura == true
                         && v.NotificacionAperturaEnviada == false)
                .ToListAsync();

            if (!votacionesPendientes.Any()) return;

            foreach (var votacion in votacionesPendientes)
            {
                var evento = votacion.Categoria?.Evento;
                if (evento == null || evento.Jurado == null) continue;

                // 2. Filtramos el jurado: Solo los que quieren recibir notificaciones (Criterio 4)
                var juecesAvisables = evento.Jurado.Where(j => j.QuiereRecibirNotificaciones).ToList();

                // 3. Generamos las notificaciones
                foreach (var juez in juecesAvisables)
                {
                    var notificacion = new Notificacion(
                        miembroId: juez.Id,
                        titulo: "¡Votación Abierta!",
                        mensaje: $"La evaluación para la categoría '{votacion.Categoria!.Name}' del evento '{evento.Name}' ha comenzado. ¡Ya puedes emitir tus votos!",
                        urlAccion: $"/juez/evento/{evento.Id}/proyectos" // Criterio 5: Link directo a la vista que hicimos antes
                    );

                    _context.Set<Notificacion>().Add(notificacion);
                }

                // 4. Actualizamos la votación para no repetir el envío en el próximo minuto (Criterio 1)
                votacion.NotificacionAperturaEnviada = true;
                votacion.Estado = "Abierta"; // ¡De paso, actualizamos su estado automáticamente!
            }

            // 5. Guardamos todos los cambios de golpe
            await _context.SaveChangesAsync();
        }
    }
}