using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models;

namespace Votify.Services.Interfaces
{
    public interface IVotoExpertoServices
    {
        Task<IEnumerable<Proyecto>> ObtenerProyectosPorCategoriaAsync(int categoriaId);
        Task GuardarComentarioAsync(int juezId, int proyectoId, int votacionId, string comentario);
        Task<IEnumerable<Voto>> ObtenerComentariosPorCategoriaAsync(int categoriaId);
    }
}
