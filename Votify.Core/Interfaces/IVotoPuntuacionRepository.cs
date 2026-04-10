using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models;

namespace Votify.Core.Interfaces
{
    public interface IVotoPuntuacionRepository
    {
        Task<Puntuacion?> ObtenerVotacionPuntuacionPorIdAsync(int votacionId);
        Task<List<Proyecto>> ObtenerProyectosPorCategoriaAsync(int categoriaId);
        Task<Votante?> ObtenerVotantePorIdAsync(int votanteId);
        Task GuardarVotosAsync(List<Voto> votos);
        Task<bool> YaVotoEnEstaVotacionAsync(int votanteId, int votacionId);
    }
}
