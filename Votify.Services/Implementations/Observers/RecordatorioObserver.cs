using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Enums;
using Votify.Core.Interfaces;
using Votify.Core.Models;

namespace Votify.Services.Implementations.Observers
{
    public class RecordatorioObserver : IVotacionStateObserver
    {
        private readonly IUnitOfWork _unitOfWork;

        public RecordatorioObserver(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(VotacionStateChangedArgs args)
        {
            if (args.EventType != VotacionStateEventType.ProximoCierre)
                return;

            Console.WriteLine($"[RECORDATORIO OBSERVER] Procesando recordatorio de votación ID: {args.Votacion.Id}");

            var evento = args.Evento;
            if (evento.Jurado == null || !evento.Jurado.Any())
            {
                Console.WriteLine($"[RECORDATORIO OBSERVER] Evento sin jurado, saliendo");
                return;
            }

            var votosSeguros = args.Votacion.Votos ?? new List<Voto>();

            var juecesPendientes = evento.Jurado
                .Where(j => j.QuiereRecibirNotificaciones &&
                           !votosSeguros.Any(v => v is VotoExperto ve && ve.JuezId == j.Id))
                .ToList();

            Console.WriteLine($"[RECORDATORIO OBSERVER] Jueces pendientes de votar: {juecesPendientes.Count}");

            foreach (var juez in juecesPendientes)
            {
                Console.WriteLine($"[RECORDATORIO OBSERVER] Creando recordatorio para juez: {juez.Name} (ID: {juez.Id})");

                var notificacion = new Notificacion(
                    miembroId: juez.Id,
                    titulo: "Queda poco tiempo para votar",
                    mensaje: $"El plazo para evaluar '{args.Votacion.Categoria!.Name}' cierra en 5 minutos. Completa tu evaluación.",
                    urlAccion: $"/juez/evento/{evento.Id}"
                );

                await _unitOfWork.Notificaciones.AddAsync(notificacion);
            }

            args.Votacion.NotificacionRecordatorioEnviada = true;
            await _unitOfWork.Votaciones.UpdateAsync(args.Votacion);

            Console.WriteLine($"[RECORDATORIO OBSERVER] Recordatorios creados para votación ID: {args.Votacion.Id}");
        }
    }
}
