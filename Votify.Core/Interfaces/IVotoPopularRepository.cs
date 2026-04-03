using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models;
using Votify.Core.Models.Votify.Core.Models;

namespace Votify.Core.Interfaces
{
    public interface IVotoPopularRepository
    {
        Task<Popular?> ObtenerVotacionPopularPorIdAsync(int votacionId);
        Task<Popular?> ObtenerPrimeraVotacionPopularDisponibleAsync();
        Task<Votante?> ObtenerVotantePorIdAsync(int votanteId);
        Task<List<Proyecto>> ObtenerProyectosPorCategoriaAsync(int categoriaId);
        Task GuardarVotosAsync(List<Voto> votos);
    }
}
