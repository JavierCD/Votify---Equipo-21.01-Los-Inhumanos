using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Services.Models.Requests;
using Votify.Services.Models.Responses;

namespace Votify.Services.Interfaces
{
    public interface IResultadosService
    {
        Task<int> CompartirClasificacionAsync(int categoriaId);
        Task<List<ResultadoIntervenidoResponse>> ObtenerResultadosPorEventoAsync(int eventoId);
        Task GuardarResultadosIntervenidosAsync(int votacionId, List<GuardarResultadoRequest> resultados);
    }
}
