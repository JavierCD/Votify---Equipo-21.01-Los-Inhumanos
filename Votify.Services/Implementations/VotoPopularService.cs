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
        private readonly IUnitOfWork _unitOfWork;

        public VotoPopularService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<VotacionPopularDisponibleResponse>> ObtenerVotacionesPopularesDisponiblesAsync()
        {
            var votaciones = await _unitOfWork.VotoPopularRepository.ObtenerVotacionesPopularesDisponiblesAsync();

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
            var votacion = await _unitOfWork.VotoPopularRepository.ObtenerVotacionPopularPorIdAsync(votacionId);

            if (votacion == null)
                throw new InvalidOperationException("La votación no existe o no está disponible.");

            var proyectos = await _unitOfWork.VotoPopularRepository.ObtenerProyectosPorCategoriaAsync(votacion.CategoriaId);

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

            var votacion = await _unitOfWork.VotoPopularRepository.ObtenerVotacionPopularPorIdAsync(request.VotacionId);

            if (votacion == null)
                throw new ArgumentException("La votación popular no existe.");

            // Evaluamos la regla de negocio pura de la máquina de estados.
            // Si la fecha actual no está dentro de la ventana de tiempo, lanzamos excepción.
            if (!votacion.PuedeVotar(DateTime.UtcNow))
                throw new InvalidOperationException("La votación no está abierta en este momento.");


            if (request.ProyectosSeleccionadosIds == null || !request.ProyectosSeleccionadosIds.Any())
                throw new ArgumentException("Debes seleccionar al menos un proyecto.");

            if (request.ProyectosSeleccionadosIds.Count > votacion.MaxSelection)
                throw new ArgumentException($"Solo puedes seleccionar hasta {votacion.MaxSelection} proyecto(s).");

            var proyectosValidos = await _unitOfWork.VotoPopularRepository.ObtenerProyectosPorCategoriaAsync(votacion.CategoriaId);
            var proyectosValidosIds = proyectosValidos.Select(p => p.Id).ToHashSet();

            if (request.ProyectosSeleccionadosIds.Any(id => !proyectosValidosIds.Contains(id)))
                throw new ArgumentException("Uno o más proyectos no pertenecen a la categoría de la votación.");

            if (!string.IsNullOrWhiteSpace(request.Email) && votacion.RestriccionVotoUnico)
            {
                bool yaVoto = await _unitOfWork.VotoPopularRepository.EmailYaVotoEnVotacionAsync(request.VotacionId, request.Email);
                if (yaVoto)
                    throw new InvalidOperationException("Este correo electrónico ya ha emitido su voto en esta votación.");
            }

            int votanteIdFinal = request.VotanteId;

            Votante votanteFinal = null;

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var todosLosVotantes = await _unitOfWork.Votantes.GetAllAsync();
                votanteFinal = todosLosVotantes.FirstOrDefault(v => v.Email == request.Email);

                if (votanteFinal == null)
                {
                    votanteFinal = new Votante
                    {
                        Email = request.Email,
                    };

                    await _unitOfWork.Votantes.AddAsync(votanteFinal);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            bool esJuez = request.JuezId.HasValue && request.JuezId.Value > 0;

            VotoCreator creadorVoto = esJuez
                ? new VotoExpertoCreator()
                : new VotoPublicoCreator();

            // Suponiendo que tienes una puntuación base que el usuario acaba de emitir
            double puntuacionBaseEmitida = 10.0; // ¡Cámbialo por la variable real si es necesario!

            List<Voto> votosAInsertar = new List<Voto>();

            // 1. Iteramos por cada proyecto que el usuario/juez ha seleccionado
            foreach (var proyectoId in request.ProyectosSeleccionadosIds)
            {
                // 2. UPSERT: Buscamos si ya existe el "cascarón" (el comentario previo)
                Voto? papeleta = votacion.Votos?.FirstOrDefault(v =>
                    v.ProyectoId == proyectoId &&
                    (
                        (esJuez && v is VotoExperto ve && ve.JuezId == request.JuezId!.Value) ||
                        (!esJuez && v is VotoPublico vp && vp.VotanteId == (votanteFinal != null ? votanteFinal.Id : request.VotanteId))
                    )
                );

                if (papeleta != null)
                {
                    // 3A. UPDATE: El voto o comentario ya existe. Le inyectamos la puntuación.
                    papeleta.PuntuacionBase = puntuacionBaseEmitida;
                }
                else
                {
                    // 3B. INSERT: No hay nada previo. Creamos el voto de cero.
                    string? hash = null;
                    if (request.Anonimo)
                    {
                        string identificadorSecreto = esJuez
                            ? request.JuezId!.Value.ToString()
                            : (!string.IsNullOrWhiteSpace(request.Email) ? request.Email : request.VotanteId.ToString());

                        hash = Convert.ToHexString(
                            SHA256.HashData(
                                Encoding.UTF8.GetBytes($"{request.VotacionId}-{identificadorSecreto}-VotifySecretSalt2026")
                            )
                        ).Substring(0, 16);
                    }

                    papeleta = creadorVoto.CrearVoto(
                        votacion.Id,
                        proyectoId,
                        puntuacionBaseEmitida,
                        request.Anonimo,
                        hash
                    );

                    if (esJuez)
                    {
                        papeleta.AsignarEmisorId(request.JuezId!.Value);
                    }
                    else if (votanteFinal != null)
                    {
                        papeleta.AsignarEmisorId(votanteFinal.Id);
                    }
                    else
                    {
                        papeleta.AsignarEmisorId(request.VotanteId);
                    }

                    votosAInsertar.Add(papeleta);
                }
            }

            // 4. Solo insertamos en base de datos los que sean totalmente nuevos
            if (votosAInsertar.Any())
            {
                await _unitOfWork.VotoPopularRepository.GuardarVotosAsync(votosAInsertar);
            }

            // Guardamos todo (tanto los Updates trackeados como los nuevos Inserts)
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<List<VotacionPopularDisponibleResponse>> ObtenerVotacionesDisponiblesAsync(int votanteId)
        {
            
            var votaciones = await _unitOfWork.VotoPopularRepository.ObtenerVotacionesPopularesDisponiblesAsync();
           

           
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
