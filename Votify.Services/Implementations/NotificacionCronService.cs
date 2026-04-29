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

                var juecesSinVotar = evento.Jurado;
                   // .Where(j => j.QuiereRecibirNotificaciones &&
                  //   !_context.Votos.Any(v => v.VotacionId == votacion.Id && v.VotanteId == j.Id))
                  //  .ToList();

                foreach (var juez in juecesSinVotar)
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
            var margenRecordatorio = TimeSpan.FromSeconds(24); // ajusta el margen que queráis

            var votacionesPendientes = await _context.Votaciones
                .Include(v => v.Categoria)
                .ThenInclude(c => c.Evento)
            .ThenInclude(e => e.Jurado)
                .Where(v => v.Estado == "Abierta"
         && v.EnviarNotificacionCierre
         && !v.NotificacionRecordatorioEnviada
         && v.FechaCierre - ahora <= margenRecordatorio
         && v.FechaCierre > ahora)
                .ToListAsync();


            if (!votacionesPendientes.Any()) return;

            foreach (var votacion in votacionesPendientes)
            {
                var evento = votacion.Categoria?.Evento;
                if (evento == null || evento.Jurado == null) continue;

                var juecesAvisables = evento.Jurado
                    .Where(j => j.QuiereRecibirNotificaciones)
                    .ToList();

                foreach (var juez in juecesAvisables)
                {
                    var notificacion = new Notificacion(
                        miembroId: juez.Id,
                        titulo: "⏰ Recordatorio: Votación próxima a cerrar",
                        mensaje: $"La evaluación para la categoría '{votacion.Categoria!.Name}' del evento '{evento.Name}' cerrará el {votacion.FechaCierre:dd/MM/yyyy HH:mm} UTC. ¡No olvides emitir tu voto!",
                        urlAccion: $"/voto-popular-usuario/{votacion.Id}"
                    );
                    _context.Set<Notificacion>().Add(notificacion);
                }

                votacion.NotificacionRecordatorioEnviada = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task ProcesarCierresDeVotacionAsync()
        {
            var ahora = DateTime.UtcNow;

            var votacionesPendientes = await _context.Votaciones
                .Include(v => v.Categoria)
                    .ThenInclude(c => c.Evento)
                        .ThenInclude(e => e.Jurado)
                .Where(v => v.FechaCierre <= ahora
                         && v.EstaCerrada == false
                         && v.NotificacionCierreEnviada == false)
                .ToListAsync();

            if (!votacionesPendientes.Any()) return;

            foreach (var votacion in votacionesPendientes)
            {
                var evento = votacion.Categoria?.Evento;
                if (evento == null || evento.Jurado == null) continue;

                // Cerrar la votación usando el método del modelo
                votacion.CerrarVotacion();
                votacion.Estado = "Cerrada";

                var juecesAvisables = evento.Jurado
                    .Where(j => j.QuiereRecibirNotificaciones)
                    .ToList();

                foreach (var juez in juecesAvisables)
                {
                    var notificacion = new Notificacion(
                        miembroId: juez.Id,
                        titulo: "🔒 Votación cerrada",
                        mensaje: $"La evaluación para la categoría '{votacion.Categoria!.Name}' del evento '{evento.Name}' ha sido cerrada definitivamente. Ya no es posible emitir nuevos votos.",
                        urlAccion: $"/voto-popular-usuario/{votacion.Id}"
                    );
                    _context.Set<Notificacion>().Add(notificacion);
                }

                votacion.NotificacionCierreEnviada = true;
            }

            await _context.SaveChangesAsync();
        }

    }
}