using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Persistence.Context;
using Microsoft.EntityFrameworkCore;


namespace Votify.Persistence.Repositories
{
    public class VotoPuntuacionRepository : IVotoPuntuacionRepository
    {
        private readonly VotifyContext _context;

        public VotoPuntuacionRepository(VotifyContext context)
        {
            _context = context;
        }

        public async Task<Puntuacion?> ObtenerVotacionPuntuacionPorIdAsync(int votacionId)
        {
            return await _context.Set<Votacion>()
                .OfType<Puntuacion>()
                .Include(v => v.Categoria)
                .FirstOrDefaultAsync(v => v.Id == votacionId);
        }

        public async Task<List<Proyecto>> ObtenerProyectosPorCategoriaAsync(int categoriaId)
        {
            return await _context.Proyectos
                .Where(p => p.Categorias.Any(c => c.Id == categoriaId))
                .ToListAsync();
        }

        public async Task<Votante?> ObtenerVotantePorIdAsync(int votanteId)
        {
            return await _context.Votantes.FindAsync(votanteId);
        }

        public async Task GuardarVotosAsync(List<Voto> votos)
        {
            _context.Votos.AddRange(votos);
        }

        public async Task<bool> YaVotoEnEstaVotacionAsync(int votanteId, int votacionId)
        {
            // Convertimos la consulta a VotoPublico
            return await _context.Votos.OfType<VotoPublico>()
                .AnyAsync(v => v.VotanteId == votanteId && v.VotacionId == votacionId);
        }
        public async Task<bool> EmailYaVotoEnVotacionAsync(int votacionId, string email)
        {
            return await _context.Votos
                .OfType<VotoPublico>()
                .AnyAsync(v => v.VotacionId == votacionId && v.Votante != null && v.Votante.Email == email);
        }
    }
}
