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
    public class AuditoriaRepository : IAuditoriaRepository
    {
        private readonly VotifyContext _context;

        public AuditoriaRepository(VotifyContext context)
        {
            _context = context;
        }

        public async Task<List<Voto>> ObtenerAuditoriaPorEventoAsync(int eventoId)
        {
            return await _context.Votos
                .Include(v => v.Proyecto)
                .Include(v => ((VotoExperto)v).Juez)
                .Include(v => ((VotoPublico)v).Votante)
                // Navegamos hacia arriba para filtrar solo los votos de este evento
                .Where(v => v.Votacion.Categoria.EventoId == eventoId)
                .OrderByDescending(v => v.Fecha)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
