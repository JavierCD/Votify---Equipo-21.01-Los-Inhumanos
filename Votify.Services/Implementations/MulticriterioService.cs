using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Interfaces;
using Votify.Services.Models;

namespace Votify.Services.Implementations
{
    public class MulticriterioService : IMulticriterioService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MulticriterioService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> CrearVotacionAsync(CrearVotacionMulticriterioRequest request)
        {
            var categoria = await _unitOfWork.Categorias.GetByIdAsync(request.CategoriaId);
            if (categoria == null) throw new Exception("La categoría no existe.");

            var votacion = new Multicriterio
            {
                CategoriaId = request.CategoriaId,
                FechaApertura = request.FechaApertura,
                FechaCierre = request.FechaCierre,
                Estado = request.Estado,
                Criterios = request.Criterios.Select(c => new Criterio
                {
                    Name = c.Nombre,
                    Peso = c.Peso
                }).ToList()
            };

            await _unitOfWork.Votaciones.AddAsync(votacion);
            await _unitOfWork.SaveChangesAsync();
            return votacion.Id;
        }
    }
}
