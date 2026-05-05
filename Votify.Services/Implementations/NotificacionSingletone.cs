using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Services.Interfaces;

namespace Votify.Services.Implementations
{
    public class NotificacionSingletone : INotificacionSingletone
    {
        public event Action<int>? OnVotacionCerrada;
        public void NotificarCierre(int votacionId)
        {
            OnVotacionCerrada?.Invoke(votacionId);
        }
    }
}
