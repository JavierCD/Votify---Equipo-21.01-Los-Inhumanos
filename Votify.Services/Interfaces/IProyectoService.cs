
using System.Threading.Tasks;
using Votify.Core.Models;
using Votify.Services.DTOs;

namespace Votify.Services.Interfaces
{
    public interface IProyectoService
    {
        Task CrearProyectoConCategoriaAsync(Proyecto proyecto, int categoriaId);
        Task<Proyecto?> ObtenerPorIdAsync(int id);
        Task<bool> ActualizarProyectoAsync(int proyectoId, int usuarioPeticionId, string rolUsuario,
            string nombre, string? descripcion, string? nombresEquipo, string? urlMateriales);
        Task<ProyectoEdicionDto?> ObtenerProyectoParaEdicionAsync(int proyectoId);
    }
}
