using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Factories;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Interfaces;

namespace Votify.Services.Implementations
{
    public class VotoExpertoService : IVotoExpertoServices
    {
        private readonly IVotoExpertoRepository _repo;

        public VotoExpertoService(IVotoExpertoRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Proyecto>> ObtenerProyectosPorCategoriaAsync(int categoriaId)
        {
            if (categoriaId <= 0)
                throw new ArgumentException("CategoriaId no válido.");

            return await _repo.ObtenerProyectosPorCategoriaAsync(categoriaId);
        }

        public async Task GuardarComentarioAsync(int juezId, int proyectoId, int votacionId, string comentario)
        {
            if (string.IsNullOrWhiteSpace(comentario))
                throw new ArgumentException("El comentario no puede estar vacío.");

            bool yaComento = await _repo.YaComentoPorProyectoAsync(juezId, proyectoId, 0);
            if (yaComento)
                throw new InvalidOperationException("Ya has dejado un comentario para este proyecto.");

            var creador = new VotoExpertoCreator();
            var voto = creador.CrearVoto(votacionId, proyectoId, 0, false, null, comentario);
            voto.AssignId(juezId);

            await _repo.GuardarComentarioAsync(voto);
        }

        public async Task<IEnumerable<Voto>> ObtenerComentariosPorCategoriaAsync(int categoriaId)
        {
            if (categoriaId <= 0)
                throw new ArgumentException("CategoriaId no válido.");

            return await _repo.ObtenerComentariosPorCategoriaAsync(categoriaId);
        }
    }
}
