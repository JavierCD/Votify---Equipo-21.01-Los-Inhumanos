using System;
using System.Collections.Generic;
using System.Security.Cryptography;
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

        public async Task GuardarComentarioAsync(int juezId, int proyectoId, int votacionId, int categoriaId, string comentario)
        {
            if (string.IsNullOrWhiteSpace(comentario))
                throw new ArgumentException("El comentario no puede estar vacío.");

            // 1. Verificación de si ya comentó
            bool yaComento = await _repo.YaComentoPorProyectoAsync(juezId, proyectoId, categoriaId);
            if (yaComento)
                throw new InvalidOperationException("Ya has dejado un comentario en este proyecto.");

            // 2. REGLAS DE NEGOCIO INTERNAS (Lo que antes le pedíamos a Blazor)
            double puntuacionBase = 0.0; // Es solo un comentario, no hay puntuación
            bool esAnonimo = false;      // Por defecto un experto no es anónimo frente al sistema (o puedes consultar la BD si lo requieres)
            string? hash = null;

            // 3. Creación del voto usando TU Factory
            var creador = new VotoExpertoCreator();
            var voto = creador.CrearVoto(votacionId, proyectoId, puntuacionBase, esAnonimo, hash, comentario);

            // 4. Asignación del Juez
            voto.AsignarEmisorId(juezId);

            // 5. Guardar
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