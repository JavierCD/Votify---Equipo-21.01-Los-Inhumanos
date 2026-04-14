using Votify.Core.Models;

namespace Votify.Core.Interfaces
{
    public interface ICategoriaService
    {
        Task<Categoria?> ObtenerPorIdAsync(int id);
        Task CrearAsync(Categoria categoria);
        Task UpdateAsync(Categoria categoria);
        Task DeleteAsync(int id);

    }
}
