using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Models;

namespace Votify.Services.Implementations
{
    public class CategoriaService : ICategoriaService
    {
        private readonly IGenericRepository<Categoria> _categoriaRepository;

        public CategoriaService(IGenericRepository<Categoria> categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        public async Task<Categoria?> ObtenerPorIdAsync(int id)
        {
            return await _categoriaRepository.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Categoria categoria)
        {
            await _categoriaRepository.UpdateAsync(categoria);
        }

        public async Task CrearAsync(Categoria categoria)
        {
            await _categoriaRepository.AddAsync(categoria);
        }

        public async Task DeleteAsync(int id)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id);
            if (categoria != null)
            {
                await _categoriaRepository.DeleteAsync(categoria.Id);
            }
        }

        public async Task AgregarPremioAsync(AgregarPremioRequest agregarPremioRequest)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(agregarPremioRequest.categoriaID);
            if(categoria != null)
            {
                
                categoria.AsignarPremio(agregarPremioRequest.nombrePremio, agregarPremioRequest.premioDesc, agregarPremioRequest.puesto);
                await _categoriaRepository.UpdateAsync(categoria);
            }

            
        }
    }
}