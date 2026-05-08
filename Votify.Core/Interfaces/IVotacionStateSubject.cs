using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models;

namespace Votify.Core.Interfaces
{
    public interface IVotacionStateSubject
    {
        void Subscribe(IVotacionStateObserver observer);
        void Unsubscribe(IVotacionStateObserver observer);
        Task NotifyAsync(VotacionStateChangedArgs args);
    }
}
