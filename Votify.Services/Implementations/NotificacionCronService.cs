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
        private readonly INotificacionSingletone _realTimeNotifier;

        public NotificacionCronService(IUnitOfWork unitOfWork, INotificacionSingletone realTimeNotifier)
        {
            _unitOfWork = unitOfWork;
            _realTimeNotifier = realTimeNotifier;
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
                    var url = $"/juez/evento/{evento.Id}";

                    var notificacion = new Notificacion(
                        miembroId: juez.Id,
                        titulo: "¡Votación Abierta!",
                        mensaje: $"La evaluación para la categoría '{votacion.Categoria!.Name}' del evento '{evento.Name}' ha comenzado. ¡Ya puedes emitir tus votos!",
                        urlAccion: url
                    );

                    await _unitOfWork.Notificaciones.AddAsync(notificacion);
                }

                votacion.NotificacionAperturaEnviada = true;
                votacion.Estado = "Abierta";
                await _unitOfWork.Votaciones.UpdateAsync(votacion);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        // 1. MÉTODO DE RECORDATORIO (Se ejecuta 2 horas antes)
        public async Task ProcesarRecordatoriosCierreAsync()
        {
            var ahora = DateTime.UtcNow;
            // Avisamos a todo lo que cierre en las próximas 2 horas
            var limiteRecordatorio = ahora.AddHours(2);

            var todasVotaciones = await _unitOfWork.Votaciones.GetAllWithIncludesAsync(
                v => v.Categoria,
                v => v.Categoria.Evento,
                v => v.Categoria.Evento.Jurado,
                v => v.Votos
            );

            var votacionesParaAvisar = todasVotaciones
                .Where(v => v.Estado == "Abierta"
                         && v.FechaCierre <= limiteRecordatorio
                         && v.FechaCierre > ahora
                         && !v.NotificacionRecordatorioEnviada)
                .ToList();

            if (!votacionesParaAvisar.Any()) return;

            foreach (var votacion in votacionesParaAvisar)
            {
                // CHIVATO PARA LA CONSOLA
                Console.WriteLine($"[CRON-RECORDATORIO] Procesando Votación ID: {votacion.Id}. Cierra a las {votacion.FechaCierre.ToLocalTime()}");

                var evento = votacion.Categoria?.Evento;
                if (evento == null || evento.Jurado == null) continue;

                var juecesAvisables = evento.Jurado.Where(j => j.QuiereRecibirNotificaciones).ToList();

                // PROTECCIÓN CONTRA NULL: Si no hay votos, creamos una lista vacía en memoria para que no explote el .Any()
                var votosSeguros = votacion.Votos ?? new List<Voto>();

                // Filtramos: Solo nos quedamos con los jueces que NO tengan un VotoExperto en esta votación
                var juecesPendientes = juecesAvisables
                    .Where(juez => !votosSeguros.Any(v => v is VotoExperto ve && ve.JuezId == juez.Id))
                    .ToList();

                Console.WriteLine($"[CRON-RECORDATORIO] Jueces totales: {juecesAvisables.Count}. Jueces que aún no votan: {juecesPendientes.Count}");

                foreach (var juez in juecesPendientes)
                {
                    Console.WriteLine($"[CRON-RECORDATORIO] ¡Enviando aviso de última hora al juez ID: {juez.Id}!");

                    var notificacion = new Notificacion(
                        miembroId: juez.Id,
                        titulo: "⏳ ¡Queda poco tiempo para votar!",
                        mensaje: $"El plazo para evaluar '{votacion.Categoria!.Name}' termina pronto. Por favor, completa tu evaluación.",
                        urlAccion: $"/juez/evento/{evento.Id}"
                    );
                    await _unitOfWork.Notificaciones.AddAsync(notificacion);
                }

                votacion.NotificacionRecordatorioEnviada = true;
                await _unitOfWork.Votaciones.UpdateAsync(votacion);
            }
            await _unitOfWork.SaveChangesAsync();
        }

        // 2. MÉTODO DE CIERRE DEFINITIVO
        public async Task ProcesarCierresDeVotacionAsync()
        {
            var ahora = DateTime.UtcNow;

            var todasVotaciones = await _unitOfWork.Votaciones.GetAllWithIncludesAsync(
                v => v.Categoria,
                v => v.Categoria.Evento,
                v => v.Categoria.Evento.Jurado
            );

            var votacionesCerradas = todasVotaciones
                .Where(v => v.Estado == "Abierta"
                         && v.FechaCierre <= ahora
                         && !v.NotificacionCierreEnviada)
                .ToList();

            if (!votacionesCerradas.Any()) return;

            foreach (var votacion in votacionesCerradas)
            {
                votacion.CerrarVotacion(); // Usa el método de dominio de tu Votacion.cs
                votacion.NotificacionCierreEnviada = true;

                var evento = votacion.Categoria?.Evento;
                if (evento != null && evento.Jurado != null)
                {
                    // Notificamos a TODOS los jueces (hayan votado o no) que se cerró
                    foreach (var juez in evento.Jurado.Where(j => j.QuiereRecibirNotificaciones))
                    {
                        var notificacion = new Notificacion(
                            miembroId: juez.Id,
                            titulo: "🔒 Votación Cerrada",
                            mensaje: $"La votación para '{votacion.Categoria!.Name}' ha finalizado.",
                            urlAccion: $"/juez/evento/{evento.Id}"
                        );
                        await _unitOfWork.Notificaciones.AddAsync(notificacion);
                    }
                }

                await _unitOfWork.Votaciones.UpdateAsync(votacion);

                // ¡EFECTO WOW! Disparamos el evento para bloquear la UI en tiempo real
                _realTimeNotifier.NotificarCierre(votacion.Id);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}