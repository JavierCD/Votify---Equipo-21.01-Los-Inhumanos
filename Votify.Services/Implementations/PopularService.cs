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
            if (!await _popularRepository.EventoExisteAsync(request.EventoId))
                throw new ArgumentException("El evento no existe.");

            if (request.FechaApertura >= request.FechaCierre)
                throw new ArgumentException("La fecha de apertura debe ser anterior a la fecha de cierre.");

            if (request.MaxSelecciones <= 0)
                throw new ArgumentException("MaxSelecciones debe ser mayor que 0.");

            if (string.IsNullOrWhiteSpace(request.Estado))
                throw new ArgumentException("El estado es obligatorio.");

            var popular = new Popular
            {
                EventoId = request.EventoId,
                FechaApertura = request.FechaApertura,
                FechaCierre = request.FechaCierre,
                Estado = request.Estado,
                MaxSelecciones = request.MaxSelecciones
            };

            var creada = await _popularRepository.CrearAsync(popular);

            return new PopularResponse
            {
                Id = creada.Id,
                EventoId = creada.EventoId,
                FechaApertura = creada.FechaApertura,
                FechaCierre = creada.FechaCierre,
                Estado = creada.Estado,
                MaxSelecciones = creada.MaxSelecciones
            };
        }
    }
}