using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models;
using Votify.Core.Interfaces;
using Votify.Persistence.Context;

namespace Votify.Persistence.Repositories
{
    public class VotoExpertoRepository:IVotoExpertoRepository
    {
        private readonly VotifyContext _context;

        public VotoExpertoRepository(VotifyContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Proyecto>> ObtenerProyectosPorCategoriaAsync(int categoriaId)
        {
            return await _context.Proyectos
                .Where(p => p.Categorias.Any(c => c.Id == categoriaId) && p.Visible)
                .ToListAsync();
        }

        public async Task GuardarComentarioAsync(Voto voto)
        {
            //voto.VotanteId = null;
            _context.Votos.Add(voto);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Voto>> ObtenerComentariosPorCategoriaAsync(int categoriaId)
        {
            // Magia: OfType<VotoExperto>() filtra automáticamente los expertos
            return await _context.Votos.OfType<VotoExperto>()
                .Include(v => v.Proyecto)
                .Where(v => v.Votacion.CategoriaId == categoriaId && v.Comentario != null)
                .ToListAsync();
        }

        public async Task<bool> YaComentoPorProyectoAsync(int juezId, int proyectoId, int categoriaId)
        {
            // Usamos OfType y buscamos directamente por JuezId
            return await _context.Votos.OfType<VotoExperto>()
                .AnyAsync(v => v.JuezId == juezId
                            && v.ProyectoId == proyectoId
                            && v.Votacion.CategoriaId == categoriaId);
        }
    }
}
