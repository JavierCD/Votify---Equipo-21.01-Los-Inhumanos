
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
                v => v.Categoria.Evento,
                v => v.Categoria.Evento.Jurado,
                v => v.Votos
            );

            Console.WriteLine($"[CRON DETECTOR] Total votaciones consultadas: {votaciones.Count()}");

            // 1. APERTURAS
            var aperturas = votaciones
                .Where(v => v.FechaApertura <= ahora
                         && v.EnviarNotificacionApertura == true
                         && v.NotificacionAperturaEnviada == false)
                .ToList();

            Console.WriteLine($"[CRON DETECTOR] Aperturas pendientes detectadas: {aperturas.Count}");

            foreach (var votacion in aperturas)
            {
                var evento = votacion.Categoria?.Evento;
                if (evento == null) continue;

                votacion.Estado = "Abierta";
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
            var recordatorios = votaciones.
                Where(v => v.Estado == "Abierta"
                        && v.FechaCierre <= limiteRecordatorio
                        && v.FechaCierre > ahora
                        && !v.NotificacionRecordatorioEnviada)
                .ToList();

            foreach (var votacion in recordatorios)
            {
                var evento = votacion.Categoria?.Evento;
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
            var cierres = votaciones
                .Where(v => v.Estado == "Abierta"
                         && v.FechaCierre <= ahora
                         && !v.NotificacionCierreEnviada)
                .ToList();

            foreach (var votacion in cierres)
            {
                var evento = votacion.Categoria?.Evento;
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
    }
}
