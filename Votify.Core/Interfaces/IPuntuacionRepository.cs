using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Interfaces
{
    public interface IPuntuacionRepository
    {
        Task<bool> CategoriaExisteAsync(int categoriaId);
        Task<Votify.Core.Models.Puntuacion> CrearAsync(Votify.Core.Models.Puntuacion puntuacion);
        Task<bool> YaExisteVotacionParaCategoriaAsync(int categoriaId);
        Task<Votify.Core.Models.Puntuacion?> ObtenerPorIdConCategoriaAsync(int votacionId);
    }
}
