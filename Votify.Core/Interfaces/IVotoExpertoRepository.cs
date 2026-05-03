using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models;


namespace Votify.Core.Interfaces
{
    public interface IVotoExpertoRepository
    {
        Task<IEnumerable<Proyecto>> ObtenerProyectosPorCategoriaAsync(int categoriaId);
        Task GuardarComentarioAsync(Voto voto);
        Task<IEnumerable<Voto>> ObtenerComentariosPorCategoriaAsync(int categoriaId);
        Task<bool> YaComentoPorProyectoAsync(int juezId, int proyectoId, int categoriaId);
        Task<IEnumerable<DetalleVoto>> ObtenerEvaluacionesPorProyectoYCriterioAsync(int proyectoId, int criterioId);
       
        Task<IEnumerable<Criterio>> ObtenerCriteriosPorProyectoAsync(int proyectoId);
   
        Task<IEnumerable<VotoExperto>> ObtenerComentariosJuezPorProyectoAsync(int proyectoId);
        Task<Dictionary<string, string>> ObtenerMapaJuecesAsync();

    }
}
