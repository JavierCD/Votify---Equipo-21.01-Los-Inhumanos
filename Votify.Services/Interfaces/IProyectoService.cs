
using Votify.Core.Models;
using System.Threading.Tasks;

namespace Votify.Core.Interfaces
{
    public interface IProyectoService
    {
        Task CrearProyectoConCategoriaAsync(Proyecto proyecto, int categoriaId);
    }
}
