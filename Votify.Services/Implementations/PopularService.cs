using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Interfaces;
using Votify.Services.Models;

namespace Votify.Services.Implementations
{
    public class PopularService : IPopularService
    {
        private readonly IPopularRepository _popularRepository;

        public PopularService(IPopularRepository popularRepository)
        {
            _popularRepository = popularRepository;
        }

        public async Task<PopularResponse> CrearVotacionPopularAsync(CrearVotacionPopularRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "La solicitud no puede ser nula.");

            if (!await _popularRepository.CategoriaExisteAsync(request.CategoriaId))
                throw new ArgumentException("La categoría no existe.");

            if (request.FechaApertura >= request.FechaCierre)
                throw new ArgumentException("La fecha de apertura debe ser anterior a la fecha de cierre.");

            if (request.MaxSelection <= 0)
                throw new ArgumentException("MaxSelection debe ser mayor que 0.");

            if (string.IsNullOrWhiteSpace(request.Estado))
                throw new ArgumentException("El estado es obligatorio.");
            if (await _popularRepository.YaExisteVotacionParaCategoriaAsync(request.CategoriaId))
                throw new InvalidOperationException("Ya existe una votacion asociada a esta categoria");

            var popular = new Popular
            {
                CategoriaId = request.CategoriaId,
                FechaApertura = request.FechaApertura,
                FechaCierre = request.FechaCierre,
                Estado = request.Estado,
                MaxSelecciones = request.MaxSelection
            };

            var creada = await _popularRepository.CrearAsync(popular);

            return new PopularResponse
            {
                Id = creada.Id,
                CategoriaId = creada.CategoriaId,
                FechaApertura = creada.FechaApertura,
                FechaCierre = creada.FechaCierre,
                Estado = creada.Estado,
                MaxSelection = creada.MaxSelecciones
            };
        }
    }
}