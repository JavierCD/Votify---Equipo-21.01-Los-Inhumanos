using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Interfaces;
using Votify.Services.Models;

namespace Votify.Services.Implementations
{
    public class PuntuacionService : IPuntuacionService
    {
        private readonly IPuntuacionRepository _puntuacionRepository;

        public PuntuacionService(IPuntuacionRepository puntuacionRepository)
        {
            _puntuacionRepository = puntuacionRepository;
        }

        public async Task<PuntuacionResponse> CrearVotacionPuntuacionAsync(CrearVotacionPuntuacionRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (!await _puntuacionRepository.CategoriaExisteAsync(request.CategoriaId))
                throw new ArgumentException("La categoría especificada no existe.");

            if (request.FechaApertura >= request.FechaCierre)
                throw new ArgumentException("La fecha de apertura debe ser anterior a la fecha de cierre.");

            if (request.ValorMax <= 0)
                throw new ArgumentException("El valor máximo de puntuación debe ser mayor que 0.");

            if (string.IsNullOrWhiteSpace(request.Estado))
                throw new ArgumentException("El estado de la votación no puede estar vacío.");

            if (await _puntuacionRepository.YaExisteVotacionParaCategoriaAsync(request.CategoriaId))
                throw new InvalidOperationException("Ya existe una votación de puntuación para esta categoría.");

            var puntuacion = new Puntuacion
            {
                CategoriaId = request.CategoriaId,
                FechaApertura = request.FechaApertura,
                FechaCierre = request.FechaCierre,
                Estado = request.Estado,
                ValorMax = request.ValorMax,
                PermiteAutoVoto=request.PermiteAutoVoto,
                RestriccionVotoUnico=request.RestriccionVotoUnico,
            };

            var resultado = await _puntuacionRepository.CrearAsync(puntuacion);

            return new PuntuacionResponse
            {
                Id = resultado.Id,
                CategoriaId = resultado.CategoriaId,
                FechaApertura = resultado.FechaApertura,
                FechaCierre = resultado.FechaCierre,
                Estado = resultado.Estado,
                ValorMax = resultado.ValorMax
            };
        }
        public async Task<VotacionPuntuacionDetalleResponse> ObtenerProyectosParaVotarAsync(int votacionId, int votanteId)
        {
            var votacion = await _puntuacionRepository.ObtenerPorIdConCategoriaAsync(votacionId);
            if (votacion == null)
                throw new ArgumentException("La votacion no existe.");

            if (votacion.Categoria == null)
                throw new InvalidOperationException("La votacion no tiene una categortia asociada");

            var proyectos = votacion.Categoria.Proyectos.ToList();
            if(!votacion.PermiteAutoVoto)
            {
                proyectos = proyectos.Where(p => p.ParticipanteId != votanteId).ToList();

            }
            return new VotacionPuntuacionDetalleResponse
            {
                VotacionId = votacion.Id,
                CategoriaId = votacion.CategoriaId,
                CategoriaNombre = votacion.Categoria.Name,
                Estado = votacion.Estado,
                ValorMax = votacion.ValorMax,
                PermiteAutoVoto = votacion.PermiteAutoVoto,
                RestriccionVotoUnico = votacion.RestriccionVotoUnico,
                Proyectos = proyectos.Select(p => new ProyectoVotacionPopularResponse
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList()

            };
        }
    }
}

