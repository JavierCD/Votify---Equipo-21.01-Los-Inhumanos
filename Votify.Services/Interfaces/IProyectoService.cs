using System.Threading.Tasks;
using Votify.Core.Models;
using Votify.Services.Models.Requests;
using Votify.Services.Models.Responses;

namespace Votify.Services.Interfaces
{
    public interface IProyectoService
    {
        Task CrearProyectoConCategoriaAsync(Proyecto proyecto, int categoriaId);

        Task<Proyecto?> ObtenerPorIdAsync(int id);

        // Cambiado para recibir el objeto Request
        Task<bool> ActualizarProyectoAsync(EditarProyectoRequest request, int usuarioPeticionId, string rolUsuario);

        // Cambiado para devolver el objeto Response
        Task<EditarProyectoResponse?> ObtenerProyectoParaEdicionAsync(int proyectoId);
    }
}