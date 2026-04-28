using System.Security.Cryptography;
using System.Text;
using Votify.Core.Factories;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Interfaces;
using Votify.Services.Models;

namespace Votify.Services.Implementations
{
    public class VotoPuntuacionService : IVotoPuntuacionService
    {
        private readonly IVotoPuntuacionRepository _votoPuntuacionRepository;
        private readonly IGenericRepository<Votante> _votanteRepository;

        public VotoPuntuacionService(IVotoPuntuacionRepository votoPuntuacionRepository)
        {
            _votoPuntuacionRepository = votoPuntuacionRepository;
        }

        public async Task<VotacionPuntuacionDetalleResponse> ObtenerDetallePorIdAsync(int votacionId)
        {
            var votacion = await _votoPuntuacionRepository.ObtenerVotacionPuntuacionPorIdAsync(votacionId);

            if (votacion == null)
                throw new InvalidOperationException("La votación no existe o no está disponible.");

            var proyectos = await _votoPuntuacionRepository.ObtenerProyectosPorCategoriaAsync(votacion.CategoriaId);

            return new VotacionPuntuacionDetalleResponse
            {
                VotacionId = votacion.Id,
                CategoriaId = votacion.CategoriaId,
                CategoriaNombre = votacion.Categoria?.Name ?? "Sin categoría",
                Estado = votacion.Estado,
                ValorMax = votacion.ValorMax,
                Proyectos = proyectos.Select(p => new ProyectoVotacionPopularResponse
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList()
            };
        }

        public async Task EmitirVotoPuntuacionAsync(EmitirVotoPuntuacionRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var votacion = await _votoPuntuacionRepository.ObtenerVotacionPuntuacionPorIdAsync(request.VotacionId);

            if (votacion == null)
                throw new ArgumentException("La votación no existe.");


            if (request.PuntuacionesPorProyecto == null || !request.PuntuacionesPorProyecto.Any())
                throw new ArgumentException("Debes puntuar al menos un proyecto.");

            if (request.PuntuacionesPorProyecto.Any(p => p.Value < 0 || p.Value > votacion.ValorMax))
                throw new ArgumentException($"Las puntuaciones deben estar entre 0 y {votacion.ValorMax}.");
            
            if (request.PuntuacionesPorProyecto.Values.Sum() > votacion.ValorMax)
                throw new ArgumentException($"La suma total de puntuaciones no puede superar {votacion.ValorMax} puntos.");

            var proyectosValidos = await _votoPuntuacionRepository.ObtenerProyectosPorCategoriaAsync(votacion.CategoriaId);
            var proyectosValidosIds = proyectosValidos.Select(p => p.Id).ToHashSet();

            if (request.PuntuacionesPorProyecto.Keys.Any(id => !proyectosValidosIds.Contains(id)))
                throw new ArgumentException("Uno o más proyectos no pertenecen a la categoría de la votación.");

            int votanteIdFinal = request.VotanteId;

            Votante votanteFinal = null;

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var todosLosVotantes = await _votanteRepository.GetAllAsync();
                votanteFinal = todosLosVotantes.FirstOrDefault(v => v.Email == request.Email);

                if (votanteFinal == null)
                {

                    votanteFinal = new Votante
                    {
                        Email = request.Email,
                    };


                    await _votanteRepository.AddAsync(votanteFinal);
                }
            }

            var creadorVoto = new VotoPublicoCreator();

            var votos = request.PuntuacionesPorProyecto.Select(kvp =>
            {
                string? hash = null;
                if (request.Anonimo)
                {
                    hash = Convert.ToHexString(
                        SHA256.HashData(
                            Encoding.UTF8.GetBytes($"{request.VotacionId}-{kvp.Key}-{DateTime.UtcNow.Ticks}")
                        )
                    ).Substring(0, 16);
                }

                var voto = creadorVoto.CrearVoto(
                    request.VotacionId,
                    kvp.Key,
                    kvp.Value,
                    request.Anonimo,
                    hash
                );

                

                return voto;
            }).ToList();

            await _votoPuntuacionRepository.GuardarVotosAsync(votos);
        }
    }
}
