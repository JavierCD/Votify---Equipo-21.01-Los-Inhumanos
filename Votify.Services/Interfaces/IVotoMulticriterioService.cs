using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Services.Models;

namespace Votify.Services.Interfaces
{
    public interface IVotoMulticriterioService
    {
        Task<List<VotacionMulticriterioDetalleResponse>> ObtenerVotacionesMulticriterioDisponiblesAsync();
        Task<VotacionMulticriterioDetalleResponse> ObtenerDetallePorIdAsync(int votacionId);
        Task EmitirVotoMulticriterioAsync(EmitirVotoMulticriterioRequest request);
        Task<List<VotacionMulticriterioDetalleResponse>> ObtenerVotacionesDisponiblesAsync(int votanteId);
    }
}
