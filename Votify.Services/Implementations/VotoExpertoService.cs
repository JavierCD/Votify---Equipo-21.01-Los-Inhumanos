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

            bool yaComento = await _unitOfWork.VotoExpertoRepository.YaComentoPorProyectoAsync(juezId, proyectoId, categoriaId);
            if (yaComento)
                throw new InvalidOperationException("Ya has dejado un comentario en este proyecto.");

            double puntuacionBase = 0.0;
            bool esAnonimo = false;
            string? hash = null;

            var creador = new VotoExpertoCreator();
            var voto = creador.CrearVoto(votacionId, proyectoId, puntuacionBase, esAnonimo, hash, comentario);

            voto.AsignarEmisorId(juezId);

            await _unitOfWork.VotoExpertoRepository.GuardarComentarioAsync(voto);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<Voto>> ObtenerComentariosPorCategoriaAsync(int categoriaId)
        {
            if (categoriaId <= 0)
                throw new ArgumentException("CategoriaId no válido.");

            return await _unitOfWork.VotoExpertoRepository.ObtenerComentariosPorCategoriaAsync(categoriaId);
        }
        public async Task<List<EvaluacionJuezResponse>> ObtenerEvaluacionesParaParticipanteAsync(int proyectoId,int categoriaId)
        {
            if (proyectoId <= 0)
                throw new ArgumentException("ProyectoId no valido.");
            if (categoriaId <= 0)
                throw new ArgumentException("CategoriaId no valido.");

            var votos = await _unitOfWork.VotoExpertoRepository.ObtenerEvaluacionesPorProyectoYCategoriaAsync(proyectoId, categoriaId);
            return votos.Select(v => new EvaluacionJuezResponse
            {
                NombreJuez = v.Juez?.Name ?? "Juez anonimo",
                Puntuacion = v.PuntuacionBase,
                Comentario = v.Comentario,
                Fecha = v.Fecha
            }).ToList();
        }
    }
}