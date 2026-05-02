using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Interfaces
{
    public interface IVotacionService
    {
        Task ActualizarFechasVotacionAsync(int votacionId, DateTime nuevaApertura, DateTime nuevoCierre);
        Task<bool> CambiarEstadoVotacionManualAsync(int votacionId, string nuevoEstado);
    }
}
