using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Persistence.Context;

namespace Votify.Persistence.Repositories
{
    public class EventoRepository : GenericRepository<Evento>, IEventoRepository
    {
        private readonly VotifyContext _context;

        public EventoRepository(VotifyContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Evento?> ObtenerEventoConDetallesAsync(int id)
        {

            return await _context.Eventos
                .Include(e => e.Participantes)
                .Include(e => e.Organizador)
                .Include(e => e.CategoriasEvento)
                    .ThenInclude(c => c.Premios)    
                .Include(e => e.CategoriasEvento)
                    .ThenInclude(c => c.Proyectos)
                        .ThenInclude(p => p.Participante)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}