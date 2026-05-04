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
        private readonly IUnitOfWork _unitOfWork;

        public VotoPuntuacionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<VotacionPuntuacionDetalleResponse> ObtenerDetallePorIdAsync(int votacionId)
        {
            var votacion = await _unitOfWork.VotoPuntuacionRepository.ObtenerVotacionPuntuacionPorIdAsync(votacionId);

            if (votacion == null)
                throw new InvalidOperationException("La votación no existe o no está disponible.");

            var proyectos = await _unitOfWork.VotoPuntuacionRepository.ObtenerProyectosPorCategoriaAsync(votacion.CategoriaId);

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

            var votacion = await _unitOfWork.VotoPuntuacionRepository.ObtenerVotacionPuntuacionPorIdAsync(request.VotacionId);

            if (votacion == null)
                throw new ArgumentException("La votación no existe.");

            if (!votacion.PuedeVotar(DateTime.UtcNow))
                throw new InvalidOperationException("La votación no está abierta en este momento.");


            if (request.PuntuacionesPorProyecto == null || !request.PuntuacionesPorProyecto.Any())
                throw new ArgumentException("Debes puntuar al menos un proyecto.");

            if (request.PuntuacionesPorProyecto.Any(p => p.Value < 0 || p.Value > votacion.ValorMax))
                throw new ArgumentException($"Las puntuaciones deben estar entre 0 y {votacion.ValorMax}.");
            
            if (request.PuntuacionesPorProyecto.Values.Sum() > votacion.ValorMax)
                throw new ArgumentException($"La suma total de puntuaciones no puede superar {votacion.ValorMax} puntos.");

            var proyectosValidos = await _unitOfWork.VotoPuntuacionRepository.ObtenerProyectosPorCategoriaAsync(votacion.CategoriaId);
            var proyectosValidosIds = proyectosValidos.Select(p => p.Id).ToHashSet();

            if (request.PuntuacionesPorProyecto.Keys.Any(id => !proyectosValidosIds.Contains(id)))
                throw new ArgumentException("Uno o más proyectos no pertenecen a la categoría de la votación.");

            if (!string.IsNullOrWhiteSpace(request.Email) && votacion.RestriccionVotoUnico)
            {
                bool yaVoto = await _unitOfWork.VotoPuntuacionRepository.EmailYaVotoEnVotacionAsync(request.VotacionId, request.Email);
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

            List<Voto> votosAInsertar = new List<Voto>();

            // 1. Iteramos por cada puntuación que el juez/usuario ha enviado
            foreach (var kvp in request.PuntuacionesPorProyecto)
            {
                int proyectoId = kvp.Key;
                double puntuacion = kvp.Value;

                // 2. UPSERT: Buscamos si ya existe el cascarón (el comentario previo)
                Voto? papeleta = votacion.Votos?.FirstOrDefault(v =>
                    v.ProyectoId == proyectoId &&
                    (
                        (esJuez && v is VotoExperto ve && ve.JuezId == request.JuezId!.Value) ||
                        (!esJuez && v is VotoPublico vp && vp.VotanteId == (votanteFinal != null ? votanteFinal.Id : request.VotanteId))
                    )
                );

                if (papeleta != null)
                {
                    // 3A. UPDATE: El voto o comentario ya existe. Solo le inyectamos la puntuación.
                    papeleta.PuntuacionBase = puntuacion;
                    // Entity Framework detectará el cambio y hará un UPDATE al hacer SaveChangesAsync
                }
                else
                {
                    // 3B. INSERT: No hay nada previo. Creamos el voto de cero.
                    string? hash = null;
                    if (request.Anonimo)
                    {
                        hash = Convert.ToHexString(
                            SHA256.HashData(
                                Encoding.UTF8.GetBytes($"{request.VotacionId}-{proyectoId}-{DateTime.UtcNow.Ticks}")
                            )
                        ).Substring(0, 16);
                    }

                    papeleta = creadorVoto.CrearVoto(
                        request.VotacionId,
                        proyectoId,
                        puntuacion,
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

            // 4. Solo insertamos los que sean totalmente nuevos
            if (votosAInsertar.Any())
            {
                await _unitOfWork.VotoPuntuacionRepository.GuardarVotosAsync(votosAInsertar);
            }

            // Guardamos todos los cambios (Updates e Inserts)
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
