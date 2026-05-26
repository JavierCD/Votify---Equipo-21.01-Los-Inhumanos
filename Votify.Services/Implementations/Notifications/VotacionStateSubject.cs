using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Implementations.Observers;

namespace Votify.Services.Implementations
{
    public class VotacionStateSubject : IVotacionStateSubject
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly RealTimeUINotificationObserver _uiObserver;

        public VotacionStateSubject(IServiceProvider serviceProvider, RealTimeUINotificationObserver uiObserver)
        {
            _serviceProvider = serviceProvider;
            _uiObserver = uiObserver;
        }

        public void Subscribe(IVotacionStateObserver observer)
        {
        }

        public void Unsubscribe(IVotacionStateObserver observer)
        {
        }

        public async Task NotifyAsync(VotacionStateChangedArgs args)
        {
            await _uiObserver.HandleAsync(args);

            using var scope = _serviceProvider.CreateScope();
            var scopedObservers = scope.ServiceProvider.GetRequiredService<IEnumerable<IVotacionStateObserver>>()
                .Where(o => o != _uiObserver);
            var tasks = scopedObservers.Select(o => o.HandleAsync(args));
            await Task.WhenAll(tasks);
        }
    }
}
