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
    public class VotoMulticriterioRepository : IVotoMulticriterioRepository
    {
        private readonly VotifyContext _context;

        public VotoMulticriterioRepository(VotifyContext context)
        {
            _context = context;
        }

        public async Task<Multicriterio?> ObtenerVotacionMulticriterioPorIdAsync(int votacionId)
        {
            // Usamos OfType<> porque Multicriterio vive en la tabla Votaciones (TPH)
            return await _context.Votaciones
                .OfType<Multicriterio>()
                .Include(v => v.Categoria)
                    .ThenInclude(c => c.Proyectos) // Traemos los proyectos de la categoría
                .Include(v => v.Criterios)         // Traemos el baremo
                .FirstOrDefaultAsync(v => v.Id == votacionId);
        }

        public async Task<List<Multicriterio>> ObtenerVotacionesMulticriterioDisponiblesAsync()
        {
            return await _context.Votaciones
                .OfType<Multicriterio>()
                .Include(v => v.Categoria)
                .Where(v => v.Estado == "Abierta")
                .ToListAsync();
        }

        public async Task<Votante?> ObtenerVotantePorIdAsync(int votanteId)
        {
            return await _context.Votantes.FindAsync(votanteId);
        }

        public async Task<List<Proyecto>> ObtenerProyectosPorCategoriaAsync(int categoriaId)
        {
            // Busca proyectos que contengan la categoría solicitada en su lista de Categorías
            return await _context.Proyectos
                .Where(p => p.Categorias.Any(c => c.Id == categoriaId))
                .ToListAsync();
        }

        public async Task GuardarVotosAsync(List<Voto> votos)
        {
            await _context.Votos.AddRangeAsync(votos);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> YaVotoEnEstaVotacionAsync(int votanteId, int votacionId)
        {
            return await _context.Votos
                .AnyAsync(v => v.VotanteId == votanteId && v.VotacionId == votacionId);
        }
        public async Task<bool> EmailYaVotoEnVotacionAsync(int votacionId, string email)
        {
            // Buscamos en Votantes para mantener la Doble Bóveda intacta
            return await _context.Votantes
                .AnyAsync(v => v.votacionId == votacionId && v.Email == email);
        }

    }
}
