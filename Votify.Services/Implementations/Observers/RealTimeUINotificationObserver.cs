
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Core.Enums;

namespace Votify.Services.Implementations.Observers
{
    public class RealTimeUINotificationObserver : IVotacionStateObserver
    {
        public event Action<int>? OnVotacionCerrada;

        public async Task HandleAsync(VotacionStateChangedArgs args)
        {
            if (args.EventType != VotacionStateEventType.Cierre)
                return;

            Console.WriteLine($"[REALTIME UI OBSERVER] Disparando OnVotacionCerrada para votación ID: {args.Votacion.Id}");
            OnVotacionCerrada?.Invoke(args.Votacion.Id);
            await Task.CompletedTask;
        }
    }
}
