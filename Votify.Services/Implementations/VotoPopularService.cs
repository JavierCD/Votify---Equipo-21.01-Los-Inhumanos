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
        private readonly IGenericRepository<Votante> _votanteRepository;

        public VotoPopularService(IVotoPopularRepository votoPopularRepository, IGenericRepository<Votante> votanteRepository)
        {
            _votoPopularRepository = votoPopularRepository;
            _votanteRepository = votanteRepository;
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

           if(!string.IsNullOrWhiteSpace(request.Email))
            {
                bool yaVoto = await _votoPopularRepository.EmailYaVotoEnVotacionAsync(request.VotacionId, request.Email);
                if (yaVoto)
                    throw new InvalidOperationException("Este correo electronico ya ha emitido su voto en esta votacion");
            }
            
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

            // Suponiendo que tienes una puntuación base que el usuario acaba de emitir (ej. 10 puntos)
            double puntuacionBaseEmitida = 10.0; // ¡Cámbialo por la variable de puntuación real de tu request!

            var votos = request.ProyectosSeleccionadosIds.Select(proyectoId =>
            {
                string? hash = null;
                if (request.Anonimo)
                {
                    // Usamos el Email si lo hay, o el ID del votante, combinado con el ID de la votación y un "Salt" secreto.
                    string identificadorSecreto = !string.IsNullOrWhiteSpace(request.Email) ? request.Email : request.VotanteId.ToString();

                    hash = Convert.ToHexString(
                        SHA256.HashData(
                            Encoding.UTF8.GetBytes($"{request.VotacionId}-{identificadorSecreto}-VotifySecretSalt2026")
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

                if (votanteFinal != null)
                {
                    voto.AsignarEmisorId(votanteFinal.Id);
                }
                else
                {
                    // Por si acaso votan sin correo, dejamos el que venía por defecto
                    voto.AsignarEmisorId(request.VotanteId);
                }



                return voto;
            }).ToList();

            await _votoPopularRepository.GuardarVotosAsync(votos);
        }
        public async Task<List<VotacionPopularDisponibleResponse>> ObtenerVotacionesDisponiblesAsync(int votanteId)
        {
            
            var votaciones = await _votoPopularRepository.ObtenerVotacionesPopularesDisponiblesAsync();
           

           
            return votaciones.Select(v => new VotacionPopularDisponibleResponse
            {
                VotacionId = v.Id,
                CategoriaId = v.CategoriaId,
                CategoriaNombre = v.Categoria?.Name ?? string.Empty,
                Estado = v.Estado,
                MaxSelection = v.MaxSelection,
                PermiteAutoVoto = v.PermiteAutoVoto,
                Proyectos = new List<ProyectoVotacionPopularResponse>() 
            }).ToList();
        }
    }
}
