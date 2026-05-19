using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Votify.Core.Interfaces;
using Votify.Core.Models;

namespace Votify.Services.Implementations
{
    public class VotacionStateSubject : IVotacionStateSubject
    {
        private readonly IServiceProvider _serviceProvider;

        public VotacionStateSubject(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Subscribe(IVotacionStateObserver observer)
        {
        }

        public void Unsubscribe(IVotacionStateObserver observer)
        {
        }

        public async Task NotifyAsync(VotacionStateChangedArgs args)
        {
            using var scope = _serviceProvider.CreateScope();
            var observers = scope.ServiceProvider.GetRequiredService<IEnumerable<IVotacionStateObserver>>();
            var tasks = observers.Select(o => o.HandleAsync(args));
            await Task.WhenAll(tasks);
        }
    }
}
