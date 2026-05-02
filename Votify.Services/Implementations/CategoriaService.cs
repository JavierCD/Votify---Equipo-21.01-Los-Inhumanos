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

        public async Task AgregarPremioAsync(AgregarPremioRequest request)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(request.categoriaID);
            if (categoria != null)
            {

                categoria.AsignarPremio(
                    request.nombrePremio,
                    request.premioDesc,
                    request.puesto,
                    request.PermiteEmpate);
                await _categoriaRepository.UpdateAsync(categoria);
            }


        }

        public async Task EliminarPremioAsync(int categoriaId, int premioId)
        {
            // 1. Recuperamos el Aggregate Root
            var categoria = await _categoriaRepository.GetByIdAsync(categoriaId);

            if (categoria == null)
            {
                throw new KeyNotFoundException($"No se encontró la categoría {categoriaId}");
            }

            // 2. Delegamos la lógica al Dominio
            categoria.EliminarPremio(premioId);

            // 3. Persistimos los cambios
            await _categoriaRepository.UpdateAsync(categoria);
        }

        public async Task CerrarVotacionAsync(int categoriaId)
        {
            // 1. Orquestación: Obtener el Aggregate Root (incluyendo la entidad Votacion)
            // Es vital usar Includes porque EF Core necesita la Votacion cargada en memoria para mutarla.
            var categoria = await _categoriaRepository.GetWithIncludesAsync(
                c => c.Id == categoriaId,
                c => c.Votacion
            );

            if (categoria == null)
            {
                throw new KeyNotFoundException($"No se encontró la categoría con ID {categoriaId}");
            }

            if (categoria.Votacion == null)
            {
                throw new InvalidOperationException($"La categoría '{categoria.Name}' no tiene una votación configurada.");
            }

            // 2. Ejecución: Delegar el comportamiento al Dominio Puro
            // La entidad Votacion se encarga de cambiar su propio estado (EstaCerrada = true, etc.)
            categoria.Votacion.CerrarVotacion();

            // 3. Orquestación: Persistir los cambios en Infraestructura
            await _categoriaRepository.UpdateAsync(categoria);
        }

        public async Task ConfigurarFechas(Categoria categoria, DateTime fechaInicio, DateTime fechaFin)
        {
            categoria.Votacion.ConfigurarFechas(fechaInicio, fechaFin);
            await _categoriaRepository.UpdateAsync(categoria);
        }

        public async Task ForzarCierre(Categoria categoria)
        {
            categoria.Votacion.ForzarCierre();
            await _categoriaRepository.UpdateAsync(categoria);

        }

        public async Task ForzarApertura(Categoria categoria)
        {
            categoria.Votacion.ForzarApertura();
            await _categoriaRepository.UpdateAsync(categoria);
        }

        public async Task PausarVotacion(Categoria categoria)
        {
            categoria.Votacion.PausarVotacion();
            await _categoriaRepository.UpdateAsync(categoria);
        }

        public async Task ForzarProgramada(Categoria categoria)
        {
            categoria.Votacion.ForzarProgramada();
            await _categoriaRepository.UpdateAsync(categoria);
        }
    }
}