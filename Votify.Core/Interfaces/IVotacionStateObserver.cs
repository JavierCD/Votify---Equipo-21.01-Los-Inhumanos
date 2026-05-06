using Votify.Core.Models;

namespace Votify.Core.Interfaces
{
    public interface IVotacionStateObserver
    {
        Task HandleAsync(VotacionStateChangedArgs args);
    }
}
