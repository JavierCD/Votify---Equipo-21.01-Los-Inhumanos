using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models;
using Votify.Services.Models;

namespace Votify.Services.Interfaces
{
    public interface IVotoExpertoServices
    {
        Task<IEnumerable<Proyecto>> ObtenerProyectosPorCategoriaAsync(int categoriaId);

        // Fíjate qué limpio queda ahora, sin parámetros basura
        Task GuardarComentarioAsync(int juezId, int proyectoId, int votacionId, int categoriaId, string comentario);

        Task<IEnumerable<Voto>> ObtenerComentariosPorCategoriaAsync(int categoriaId);
        Task<List<EvaluacionJuezResponse>> ObtenerEvaluacionesParaParticipanteAsync(int proyectoId, int criterioId);
       
        Task<List<Criterio>> ObtenerCriteriosPorProyectoAsync(int proyectoId);
       
        Task<List<EvaluacionJuezResponse>> ObtenerComentariosJuezPorProyectoAsync(int proyectoId);
    }
}
