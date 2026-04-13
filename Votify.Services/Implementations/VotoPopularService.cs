using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Votify.Core.Factories;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Interfaces;
using Votify.Services.Models;

namespace Votify.Services.Implementations
{
    public class VotoPopularService : IVotoPopularService
    {
        private readonly IVotoPopularRepository _votoPopularRepository;

        public VotoPopularService(IVotoPopularRepository votoPopularRepository)
        {
            _votoPopularRepository = votoPopularRepository;
        }

        public async Task<List<VotacionPopularDisponibleResponse>> ObtenerVotacionesPopularesDisponiblesAsync()
        {
            var votaciones = await _votoPopularRepository.ObtenerVotacionesPopularesDisponiblesAsync();

            return votaciones.Select(v => new VotacionPopularDisponibleResponse
            {
                VotacionId = v.Id,
                CategoriaId = v.CategoriaId,
                CategoriaNombre = v.Categoria?.Name ?? "Sin categoría",
                Estado = v.Estado,
                MaxSelection = v.MaxSelection,
                Proyectos = new() 
            }).ToList();
        }

        public async Task<VotacionPopularDisponibleResponse> ObtenerDetallePorIdAsync(int votacionId)
        {
            var votacion = await _votoPopularRepository.ObtenerVotacionPopularPorIdAsync(votacionId);

            if (votacion == null)
                throw new InvalidOperationException("La votación no existe o no está disponible.");

            var proyectos = await _votoPopularRepository.ObtenerProyectosPorCategoriaAsync(votacion.CategoriaId);

            return new VotacionPopularDisponibleResponse
            {
                VotacionId = votacion.Id,
                CategoriaId = votacion.CategoriaId,
                CategoriaNombre = votacion.Categoria?.Name ?? "Sin categoría",
                Estado = votacion.Estado,
                MaxSelection = votacion.MaxSelection,
                Proyectos = proyectos.Select(p => new ProyectoVotacionPopularResponse
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList()
            };
        }

        public async Task EmitirVotoPopularAsync(EmitirVotoPopularRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var votacion = await _votoPopularRepository.ObtenerVotacionPopularPorIdAsync(request.VotacionId);

            if (votacion == null)
                throw new ArgumentException("La votación popular no existe.");

            
            if (request.ProyectosSeleccionadosIds == null || !request.ProyectosSeleccionadosIds.Any())
                throw new ArgumentException("Debes seleccionar al menos un proyecto.");

            if (request.ProyectosSeleccionadosIds.Count > votacion.MaxSelection)
                throw new ArgumentException($"Solo puedes seleccionar hasta {votacion.MaxSelection} proyecto(s).");

            var proyectosValidos = await _votoPopularRepository.ObtenerProyectosPorCategoriaAsync(votacion.CategoriaId);
            var proyectosValidosIds = proyectosValidos.Select(p => p.Id).ToHashSet();

            if (request.ProyectosSeleccionadosIds.Any(id => !proyectosValidosIds.Contains(id)))
                throw new ArgumentException("Uno o más proyectos no pertenecen a la categoría de la votación.");

            var creadorVoto = new VotoPublicoCreator();

            // Suponiendo que tienes una puntuación base que el usuario acaba de emitir (ej. 10 puntos)
            double puntuacionBaseEmitida = 10.0; // ¡Cámbialo por la variable de puntuación real de tu request!

            // 2. Ahora usamos la fábrica elegida para crear cada voto
            var votos = request.ProyectosSeleccionadosIds.Select(proyectoId =>
            {
                string? hash = null;
                if (request.Anonimo)
                {
                    hash = Convert.ToHexString(
                        SHA256.HashData(
                            Encoding.UTF8.GetBytes($"{request.VotacionId}-{proyectoId}-{DateTime.UtcNow.Ticks}")
                        )
                    ).Substring(0, 16);
                }

                var voto = creadorVoto.CrearVoto(
                    votacion.Id,
                    proyectoId,
                    puntuacionBaseEmitida,
                    request.Anonimo,
                    hash
                );

            

                return voto;
            }).ToList();

            await _votoPopularRepository.GuardarVotosAsync(votos);
        }
    }
}
