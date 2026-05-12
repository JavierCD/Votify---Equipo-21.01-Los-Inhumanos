using Votify.Core.Enums;
using Votify.Core.Interfaces;
using Votify.Core.Models;

namespace Votify.Services.Implementations.Observers
{
    public class CierreNotificationObserver : IVotacionStateObserver
    {
        private readonly IUnitOfWork _unitOfWork;

        public CierreNotificationObserver(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(VotacionStateChangedArgs args)
        {
            if (args.EventType != VotacionStateEventType.Cierre)
                return;

            Console.WriteLine($"[CIERRE OBSERVER] Procesando cierre de votación ID: {args.Votacion.Id}");

            var evento = args.Evento;
            if (evento.Jurado == null || !evento.Jurado.Any())
            {
                Console.WriteLine($"[CIERRE OBSERVER] Evento sin jurado, saliendo");
                return;
            }

            Console.WriteLine($"[CIERRE OBSERVER] Jurado encontrado: {evento.Jurado.Count()} jueces");

            var jueces = evento.Jurado
                .Where(j => j.QuiereRecibirNotificaciones)
                .ToList();

            Console.WriteLine($"[CIERRE OBSERVER] Jueces que quieren notificaciones: {jueces.Count}");

            foreach (var juez in jueces)
            {
                Console.WriteLine($"[CIERRE OBSERVER] Creando notificación de cierre para juez: {juez.Name} (ID: {juez.Id})");

                var notificacion = new Notificacion(
                    miembroId: juez.Id,
                    titulo: "Votación Cerrada",
                    mensaje: $"La votación para la categoría '{args.Votacion.Categoria!.Name}' del evento '{evento.Name}' ha finalizado.",
                    urlAccion: $"/juez/evento/{evento.Id}"
                );

                await _unitOfWork.Notificaciones.AddAsync(notificacion);
            }

            args.Votacion.CerrarVotacion();
            args.Votacion.NotificacionCierreEnviada = true;
            await _unitOfWork.Votaciones.UpdateAsync(args.Votacion);

            Console.WriteLine($"[CIERRE OBSERVER] Notificaciones de cierre creadas y votación cerrada ID: {args.Votacion.Id}");
        }
    }

}
