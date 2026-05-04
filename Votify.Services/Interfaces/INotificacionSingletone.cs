using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Interfaces
{
    public interface INotificacionSingletone
    {
        event Action<int>? OnVotacionCerrada;
        void NotificarCierre(int votacionId);
    }
}
