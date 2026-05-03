
using Votify.Core.Models;
using System.Threading.Tasks;

namespace Votify.Services.Interfaces
{
    public interface IProyectoService
    {
        Task CrearProyectoConCategoriaAsync(Proyecto proyecto, int categoriaId);
        Task<Proyecto?> ObtenerPorIdAsync(int id);
    }
}
