using Votify.Core.Enums;
using Votify.Core.Interfaces;
using Votify.Core.Models;

namespace Votify.Services.Implementations.Observers
{
    public class AperturaNotificationObserver : IVotacionStateObserver
    {
        private readonly IUnitOfWork _unitOfWork;

        public AperturaNotificationObserver(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(VotacionStateChangedArgs args)
        {
            if (args.EventType != VotacionStateEventType.Apertura)
                return;

            Console.WriteLine($"[APERTURA OBSERVER] Procesando votación ID: {args.Votacion.Id}");

            if (!args.Votacion.EnviarNotificacionApertura)
            {
                Console.WriteLine($"[APERTURA OBSERVER] EnviarNotificacionApertura = false, saliendo");
                return;
            }

            var evento = args.Evento;
            if (evento.Jurado == null || !evento.Jurado.Any())
            {
                Console.WriteLine($"[APERTURA OBSERVER] Evento sin jurado, saliendo");
                return;
            }

            Console.WriteLine($"[APERTURA OBSERVER] Jurado encontrado: {evento.Jurado.Count()} jueces");

            var jueces = evento.Jurado
                .Where(j => j.QuiereRecibirNotificaciones)
                .ToList();

            Console.WriteLine($"[APERTURA OBSERVER] Jueces que quieren notificaciones: {jueces.Count}");

            foreach (var juez in jueces)
            {
                Console.WriteLine($"[APERTURA OBSERVER] Creando notificación para juez: {juez.Name} (ID: {juez.Id})");

                var notificacion = new Notificacion(
                    miembroId: juez.Id,
                    titulo: "Votación abierta",
                    mensaje: $"La evaluación para la categoría '{args.Votacion.Categoria!.Name}' del evento '{evento.Name}' ha comenzado. ¡Ya puedes emitir tus votos!",
                    urlAccion: $"/juez/evento/{evento.Id}"
                    );

                await _unitOfWork.Notificaciones.AddAsync(notificacion);
            }

            args.Votacion.NotificacionAperturaEnviada = true;
            await _unitOfWork.Votaciones.UpdateAsync(args.Votacion);

            Console.WriteLine($"[APERTURA OBSERVER] Notificaciones creadas y flag marcado para votación ID: {args.Votacion.Id}");
        }
    }
}
