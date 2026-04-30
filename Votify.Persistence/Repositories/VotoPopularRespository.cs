using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Persistence.Context;

namespace Votify.Persistence.Repositories
{
    public class VotoPopularRepository : IVotoPopularRepository
    {
        private readonly VotifyContext _context;

        public VotoPopularRepository(VotifyContext context)
        {
            _context = context;
        }

        public async Task<Popular?> ObtenerVotacionPopularPorIdAsync(int votacionId)
        {
            return await _context.Votaciones
                .OfType<Popular>()
                .Include(v => v.Categoria)
                .FirstOrDefaultAsync(v => v.Id == votacionId);
        }

        public async Task<List<Popular>> ObtenerVotacionesPopularesDisponiblesAsync()
        {
            return await _context.Votaciones
         .OfType<Popular>()
         .Include(v => v.Categoria)
         .Where(v => v.Estado == "Abierta")
         .ToListAsync();
        }

        public async Task<Votante?> ObtenerVotantePorIdAsync(int votanteId)
        {
            return await _context.Votantes
                .FirstOrDefaultAsync(v => v.Id == votanteId);
        }

        public async Task<List<Proyecto>> ObtenerProyectosPorCategoriaAsync(int categoriaId)
        {
            return await _context.Proyectos
                .Include(p => p.Categorias)
                .Where(p => p.Categorias.Any(c => c.Id == categoriaId))
                .ToListAsync();
        }

        public async Task GuardarVotosAsync(List<Voto> votos)
        {
            _context.Votos.AddRange(votos);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> YaVotoEnEstaVotacionAsync(int votanteId, int votacionId)
        {
            return await _context.Votos
                .OfType<VotoPublico>()
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
