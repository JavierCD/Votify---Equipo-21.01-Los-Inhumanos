using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Factories;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Interfaces;
using Votify.Services.Models;

namespace Votify.Services.Implementations
{
    public class VotoExpertoService : IVotoExpertoServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public VotoExpertoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Proyecto>> ObtenerProyectosPorCategoriaAsync(int categoriaId)
        {
            if (categoriaId <= 0)
                throw new ArgumentException("CategoriaId no válido.");

            return await _unitOfWork.VotoExpertoRepository.ObtenerProyectosPorCategoriaAsync(categoriaId);
        }

        public async Task GuardarComentarioAsync(int juezId, int proyectoId, int votacionId, int categoriaId, string comentario)
        {
            if (string.IsNullOrWhiteSpace(comentario))
                throw new ArgumentException("El comentario no puede estar vacío.");

            // 1. Buscamos si ya existe el registro (porque el juez pudo haber votado primero)
            var votoExistente = await _unitOfWork.VotoExpertoRepository.ObtenerVotoExpertoAsync(juezId, proyectoId, votacionId);

            if (votoExistente != null)
            {
                // 2. UPDATE: Ya existe. Solo le inyectamos/actualizamos el comentario.
                votoExistente.Comentario = comentario;
                // Al estar trackeado por Entity Framework, el SaveChangesAsync de abajo hará un UPDATE automático.
            }
            else
            {
                // 3. INSERT: No existe nada aún. Creamos el voto de cero en modo "cascarón".
                double puntuacionBase = 0.0;
                bool esAnonimo = false;
                string? hash = null;

                if (esAnonimo)
                {
                    hash = Convert.ToHexString(
                        SHA256.HashData(
                            Encoding.UTF8.GetBytes($"{votacionId}-{juezId}-VotifySecretSalt2026")
                        )
                    ).Substring(0, 16);
                }

                var creador = new VotoExpertoCreator();
                var nuevoVoto = creador.CrearVoto(votacionId, proyectoId, puntuacionBase, esAnonimo, hash, comentario);

                nuevoVoto.AsignarEmisorId(juezId);

                // Guardamos el nuevo cascarón (Asegúrate de que este método en tu repo hace un .Add())
                await _unitOfWork.VotoExpertoRepository.GuardarComentarioAsync(nuevoVoto);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<Voto>> ObtenerComentariosPorCategoriaAsync(int categoriaId)
        {
            if (categoriaId <= 0)
                throw new ArgumentException("CategoriaId no válido.");

            return await _unitOfWork.VotoExpertoRepository.ObtenerComentariosPorCategoriaAsync(categoriaId);
        }

        public async Task<List<EvaluacionJuezResponse>> ObtenerEvaluacionesParaParticipanteAsync(int proyectoId, int criterioId)
        {
            if (proyectoId <= 0)
                throw new ArgumentException("ProyectoId no válido.");
            if (criterioId <= 0)
                throw new ArgumentException("CriterioId no válido.");

            var detalles = await _unitOfWork.VotoExpertoRepository.ObtenerEvaluacionesPorProyectoYCriterioAsync(proyectoId, criterioId);
            var comentariosJuez = await _unitOfWork.VotoExpertoRepository.ObtenerComentariosJuezPorProyectoAsync(proyectoId);

            
            var comentariosPorJuezId = comentariosJuez
                .Where(c => c.JuezId.HasValue)
                .GroupBy(c => c.JuezId!.Value)
                .ToDictionary(g => g.Key, g => g.First().Comentario);

            return detalles.Select(d =>
            {
                var ve = d.Voto as VotoExperto;
                string? comentario = null;

                if (ve?.JuezId.HasValue == true)
                {
                    comentariosPorJuezId.TryGetValue(ve.JuezId!.Value, out comentario);
                }

                return new EvaluacionJuezResponse
                {
                    NombreJuez = ve?.Juez?.Name ?? "Juez anónimo",
                    Puntuacion = d.Puntuacion,
                    Comentario = comentario,
                    Fecha = d.Voto.Fecha
                };
            }).ToList();
        }
        public async Task<List<Criterio>> ObtenerCriteriosPorProyectoAsync(int proyectoId)
        {
            if (proyectoId <= 0)
                throw new ArgumentException("ProyectoId no válido.");

            var criterios = await _unitOfWork.VotoExpertoRepository.ObtenerCriteriosPorProyectoAsync(proyectoId);
            return criterios.ToList();
        }
      
        public async Task<List<EvaluacionJuezResponse>> ObtenerComentariosJuezPorProyectoAsync(int proyectoId)
        {
            if (proyectoId <= 0)
                throw new ArgumentException("ProyectoId no válido.");

           

            var votos = await _unitOfWork.VotoExpertoRepository.ObtenerComentariosJuezPorProyectoAsync(proyectoId);
            return votos.Select(v => new EvaluacionJuezResponse
            {
                NombreJuez = v.ObtenerIdentificadorAuditoria(),
                Puntuacion = v.PuntuacionBase,
                Comentario = v.Comentario,
                Fecha = v.Fecha
            }).ToList();
        }
    }
}