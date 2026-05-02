using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Models;

namespace Votify.Services.Implementations
{
    public class CategoriaService : ICategoriaService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Categoria?> ObtenerPorIdAsync(int id)
        {
            return await _unitOfWork.Categorias.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Categoria categoria)
        {
            await _unitOfWork.Categorias.UpdateAsync(categoria);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CrearAsync(Categoria categoria)
        {
            await _unitOfWork.Categorias.AddAsync(categoria);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var categoria = await _unitOfWork.Categorias.GetByIdAsync(id);
            if (categoria != null)
            {
                await _unitOfWork.Categorias.DeleteAsync(categoria.Id);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task AgregarPremioAsync(AgregarPremioRequest request)
        {
            var categoria = await _unitOfWork.Categorias.GetByIdAsync(request.categoriaID);
            if (categoria != null)
            {

                categoria.AsignarPremio(
                    request.nombrePremio,
                    request.premioDesc,
                    request.puesto,
                    request.PermiteEmpate);
                await _unitOfWork.Categorias.UpdateAsync(categoria);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task EliminarPremioAsync(int categoriaId, int premioId)
        {
            var categoria = await _unitOfWork.Categorias.GetByIdAsync(categoriaId);

            if (categoria == null)
            {
                throw new KeyNotFoundException($"No se encontró la categoría {categoriaId}");
            }

            categoria.EliminarPremio(premioId);

            await _unitOfWork.Categorias.UpdateAsync(categoria);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CerrarVotacionAsync(int categoriaId)
        {
            var categoria = await _unitOfWork.CategoriaRepository.ObtenerCategoriaConVotacionYVotosAsync(categoriaId);

            if (categoria == null)
            {
                throw new KeyNotFoundException($"No se encontró la categoría con ID {categoriaId}");
            }

            if (categoria.Votacion == null)
            {
                throw new InvalidOperationException($"La categoría '{categoria.Name}' no tiene una votación configurada.");
            }

            categoria.Votacion.CerrarVotacion();

            await _unitOfWork.Categorias.UpdateAsync(categoria);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ConfigurarFechas(Categoria categoria, DateTime fechaInicio, DateTime fechaFin)
        {
            categoria.Votacion.ConfigurarFechas(fechaInicio, fechaFin);
            await _unitOfWork.Categorias.UpdateAsync(categoria);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ForzarCierre(Categoria categoria)
        {
            categoria.Votacion.ForzarCierre();
            await _unitOfWork.Categorias.UpdateAsync(categoria);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ForzarApertura(Categoria categoria)
        {
            categoria.Votacion.ForzarApertura();
            await _unitOfWork.Categorias.UpdateAsync(categoria);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task PausarVotacion(Categoria categoria)
        {
            categoria.Votacion.PausarVotacion();
            await _unitOfWork.Categorias.UpdateAsync(categoria);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ForzarProgramada(Categoria categoria)
        {
            categoria.Votacion.ForzarProgramada();
            await _unitOfWork.Categorias.UpdateAsync(categoria);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}