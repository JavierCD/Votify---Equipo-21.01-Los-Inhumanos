using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
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

                var juecesAvisables = evento.Jurado.Where(j => j.QuiereRecibirNotificaciones).ToList();

                foreach (var juez in juecesAvisables)
                {
                    var notificacion = new Notificacion(
                        miembroId: juez.Id,
                        titulo: "¡Votación Abierta!",
                        mensaje: $"La evaluación para la categoría '{votacion.Categoria!.Name}' del evento '{evento.Name}' ha comenzado. ¡Ya puedes emitir tus votos!",
                        urlAccion: $"/juez/evento/{evento.Id}/proyectos"
                    );

                    _context.Set<Notificacion>().Add(notificacion);
                }

                votacion.NotificacionAperturaEnviada = true;
                votacion.Estado = "Abierta";
            }

            await _context.SaveChangesAsync();
        }
    }
}