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
                        urlAccion: $"/voto-popular-usuario/{votacion.Id}"
                    );

                    _context.Set<Notificacion>().Add(notificacion);
                }

                votacion.NotificacionAperturaEnviada = true;
                votacion.Estado = "Abierta";
            }

            await _context.SaveChangesAsync();
        }
        public async Task ProcesarRecordatoriosCierreAsync()
        {
            var ahora = DateTime.UtcNow;
            var margen = TimeSpan.FromMinutes(30); // puedes cambiarlo

            var votaciones = await _context.Votaciones
                .Include(v => v.Categoria)
                    .ThenInclude(c => c.Evento)
                        .ThenInclude(e => e.Jurado)
                .Where(v => v.Estado == "Abierta"
                         && v.FechaCierre <= ahora.Add(margen)
                         && v.RecordatorioCierreEnviado == false)
                .ToListAsync();

            foreach (var votacion in votaciones)
            {
                var evento = votacion.Categoria?.Evento;
                if (evento == null) continue;

                foreach (var juez in evento.Jurado.Where(j => j.QuiereRecibirNotificaciones))
                {
                    // ⚠️ AQUÍ deberías comprobar si ya ha votado
                    bool yaHaVotado = await _context.Votos
                        .AnyAsync(v => v.VotacionId == votacion.Id && v.Votante.Id == juez.Id);

                    if (yaHaVotado) continue;

                    var notificacion = new Notificacion(
                        juez.Id,
                        "⏰ Recordatorio de votación",
                        $"La votación de '{votacion.Categoria!.Name}' está a punto de cerrarse y aún no has votado.",
                        $"/voto-popular-usuario/{votacion.Id}"
                    );

                    _context.Set<Notificacion>().Add(notificacion);
                }

                votacion.RecordatorioCierreEnviado = true;
            }

            await _context.SaveChangesAsync();
        }
        public async Task ProcesarCierresDeVotacionAsync()
        {
            var ahora = DateTime.UtcNow;

            var votaciones = await _context.Votaciones
                .Include(v => v.Categoria)
                    .ThenInclude(c => c.Evento)
                        .ThenInclude(e => e.Jurado)
                .Where(v => v.Estado == "Abierta"
                         && v.FechaCierre <= ahora)
                .ToListAsync();

            foreach (var votacion in votaciones)
            {
                var evento = votacion.Categoria?.Evento;
                if (evento == null) continue;

                foreach (var miembro in evento.Jurado)
                {
                    var notificacion = new Notificacion(
                        miembro.Id,
                        "🔒 Votación cerrada",
                        $"La votación de '{votacion.Categoria!.Name}' ha finalizado.",
                        ""
                    );

                    _context.Set<Notificacion>().Add(notificacion);
                }

                votacion.Estado = "Cerrada";
            }


            await _context.SaveChangesAsync();
        }
    }
}