using Votify.Core.Models;
namespace Votify.Core.Interfaces
{
    public interface IPopularRepository
    {
        Task<bool> CategoriaExisteAsync(int categoriaId);
        Task<Popular> CrearAsync(Popular popular);
        Task<bool> YaExisteVotacionParaCategoriaAsync(int categoriaId);
        Task<Popular?> ObtenerPorIdConCategoriaAsync(int votacionId);
    }
}
