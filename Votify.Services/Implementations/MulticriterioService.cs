using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Interfaces;
using Votify.Services.Models;

namespace Votify.Services.Implementations
{
    public class MulticriterioService : IMulticriterioService
    {
        private readonly IGenericRepository<Multicriterio> _repository;
        private readonly IGenericRepository<Categoria> _categoriaRepository;

        public MulticriterioService(
            IGenericRepository<Multicriterio> repository,
            IGenericRepository<Categoria> categoriaRepository)
        {
            _repository = repository;
            _categoriaRepository = categoriaRepository;
        }

        public async Task<int> CrearVotacionAsync(CrearVotacionMulticriterioRequest request)
        {
            // 1. Validar que la categoría existe
            var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId);
            if (categoria == null) throw new Exception("La categoría no existe.");

            // 2. Mapear Request a Entidad de Dominio
            var votacion = new Multicriterio
            {
                CategoriaId = request.CategoriaId,
                FechaApertura = request.FechaApertura,
                FechaCierre = request.FechaCierre,
                Estado = request.Estado,
                // Mapeamos los criterios del DTO a la entidad Criterio
                Criterios = request.Criterios.Select(c => new Criterio
                {
                    Name = c.Nombre,
                    Peso = c.Peso
                }).ToList()
            };

            // 3. Persistir en la base de datos
            await _repository.AddAsync(votacion);
            return votacion.Id;
        }
    }
}
