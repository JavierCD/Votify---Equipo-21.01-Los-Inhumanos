using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Votify.Core.Enums;

namespace Votify.Services.Interfaces
{
    public interface IVotacionService
    {
        Task ActualizarFechasVotacionAsync(int votacionId, DateTime nuevaApertura, DateTime nuevoCierre);
        Task<bool> CambiarEstadoVotacionManualAsync(int votacionId, EstadoVotacion nuevoEstado);
        Task<bool> ActualizarVisibilidadVotacionAsync(int votacionId, bool mostrarJueces, bool mostrarComentarios, bool mostrarRanking, bool mostrarDetalles);
    }
}
