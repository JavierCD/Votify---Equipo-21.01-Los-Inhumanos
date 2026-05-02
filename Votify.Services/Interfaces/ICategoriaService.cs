using Votify.Core.Models;
using Votify.Services.Models;

namespace Votify.Core.Interfaces
{
    public interface ICategoriaService
    {
        Task<Categoria?> ObtenerPorIdAsync(int id);
        Task CrearAsync(Categoria categoria);
        Task UpdateAsync(Categoria categoria);
        Task DeleteAsync(int id);

        Task AgregarPremioAsync(AgregarPremioRequest agregarPremioRequest);
        Task EliminarPremioAsync(int categoriaId, int premioId);
        Task CerrarVotacionAsync(int categoriaId);
        Task ConfigurarFechas(Categoria categoria, DateTime fechaInicio, DateTime fechaFin);

        Task ForzarCierre(Categoria categoria);
        Task ForzarApertura(Categoria categoria);
        Task PausarVotacion(Categoria categoria);
        Task ForzarProgramada(Categoria categoria);
    }
}
