
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Core.Enums;

namespace Votify.Services.Implementations
{
    public class VotacionStateCronDetector
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVotacionStateSubject _subject;

        public VotacionStateCronDetector(IUnitOfWork unitOfWork, IVotacionStateSubject subject)
        {
            _unitOfWork = unitOfWork;
            _subject = subject;
        }

        public async Task DetectAndNotifyAsync()
        {
            var ahora = DateTime.UtcNow;
            Console.WriteLine($"[CRON DETECTOR] Ejecutando detección a las {ahora:HH:mm:ss}");

            var votaciones = await _unitOfWork.Votaciones.GetAllWithIncludesAsync(
                v => v.Categoria,
                v => v.Votos
            );

            var votacionesValidas = votaciones.Where(v => v.Categoria != null).ToList();

            Console.WriteLine($"[CRON DETECTOR] Total votaciones válidas: {votacionesValidas.Count}");

            var eventoCache = new Dictionary<int, Evento?>();

            // 1. APERTURAS
            var aperturas = votacionesValidas
                .Where(v => v.FechaApertura <= ahora
                         && v.EnviarNotificacionApertura == true
                         && v.NotificacionAperturaEnviada == false)
                .ToList();

            Console.WriteLine($"[CRON DETECTOR] Aperturas pendientes detectadas: {aperturas.Count}");

            foreach (var votacion in aperturas)
            {
                var evento = await GetEventoCached(votacion.Categoria!.EventoId, eventoCache);
                if (evento == null) continue;

                votacion.EvaluarEstadoTemporal(ahora);
                await _unitOfWork.Votaciones.UpdateAsync(votacion);

                await _subject.NotifyAsync(new VotacionStateChangedArgs
                {
                    Votacion = votacion,
                    Evento = evento,
                    EventType = VotacionStateEventType.Apertura,
                    TriggeredAt = ahora
                });
            }

            // 2. RECORDATORIOS DE CIERRE PRÓXIMO
            var limiteRecordatorio = ahora.AddMinutes(5);
            var recordatorios = votacionesValidas.
                Where(v => v.Estado == "Abierta"
                        && v.FechaCierre <= limiteRecordatorio
                        && v.FechaCierre > ahora
                        && !v.NotificacionRecordatorioEnviada)
                .ToList();

            foreach (var votacion in recordatorios)
            {
                var evento = await GetEventoCached(votacion.Categoria!.EventoId, eventoCache);
                if (evento == null) continue;

                await _subject.NotifyAsync(new VotacionStateChangedArgs
                {
                    Votacion = votacion,
                    Evento = evento,
                    EventType = VotacionStateEventType.ProximoCierre,
                    TriggeredAt = ahora
                });
            }

            // 3. CIERRES
            var cierres = votacionesValidas
                .Where(v => v.Estado == "Abierta"
                         && v.FechaCierre <= ahora
                         && !v.NotificacionCierreEnviada)
                .ToList();

            foreach (var votacion in cierres)
            {
                var evento = await GetEventoCached(votacion.Categoria!.EventoId, eventoCache);
                if (evento == null) continue;


                await _subject.NotifyAsync(new VotacionStateChangedArgs
                {
                    Votacion = votacion,
                    Evento = evento,
                    EventType = VotacionStateEventType.Cierre,
                    TriggeredAt = ahora
                });
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<Evento?> GetEventoCached(int eventoId, Dictionary<int, Evento?> cache)
        {
            if (cache.TryGetValue(eventoId, out var evento))
                return evento;

            evento = await _unitOfWork.Eventos.GetWithIncludesAsync(
                e => e.Id == eventoId,
                e => e.Jurado
            );

            cache[eventoId] = evento;
            return evento;
        }
    }
}
