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
                .Include(e => e.Jurado)
                .Include(e => e.CategoriasEvento)
                    .ThenInclude(c => c.Votacion)
                        .ThenInclude(v => v.Votos)
                            .ThenInclude(v => (v as VotoPublico)!.Votante)
                .Include(e => e.CategoriasEvento)
                    .ThenInclude(c => c.Votacion)
                        .ThenInclude(v => v.Votos)
                            .ThenInclude(v => v.Detalles)
                .Include(e => e.CategoriasEvento)
                    .ThenInclude(c => c.Votacion)
                        .ThenInclude(v => ((Multicriterio)v).Criterios)
                .Include(e => e.CategoriasEvento)
                    .ThenInclude(c => c.Proyectos)
                        .ThenInclude(p => p.Participante)
                .Include(e => e.CategoriasEvento)
                    .ThenInclude(c => c.Proyectos)
                        .ThenInclude(p => p.Votos)
                            .ThenInclude(v => v.Detalles)
                .AsSplitQuery()
                .FirstOrDefaultAsync(e => e.Id == id);
        }
        public async Task<IEnumerable<Evento>> ObtenerEventosPorJuezAsync(int juezId)
        {
            return await _context.Eventos
                .Include(e=>e.Jurado)
                .Include(e => e.CategoriasEvento)
                .ThenInclude(c => c.Proyectos)
                .Where(e => e.Jurado.Any(j => j.Id == juezId))
               .ToListAsync();
        }

        public async Task<IEnumerable<Evento>> ObtenerEventosDisponiblesAsync()
        {
            // Obtenemos eventos activos (FechaFin superior a hoy) para mostrarlos en el Dashboard
            return await _context.Eventos
                .Include(e => e.CategoriasEvento)
                .Where(e => e.FechaFin >= DateTime.UtcNow)
                .OrderBy(e => e.FechaInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<Evento>> ObtenerEventosPorParticipanteAsync(int participanteId)
        {
            // Buscamos eventos donde alguna de sus categorías contenga un proyecto del participante
            return await _context.Eventos
                .Include(e => e.CategoriasEvento)
                    .ThenInclude(c => c.Proyectos)
                .Where(e => e.CategoriasEvento.Any(c => c.Proyectos.Any(p => p.ParticipanteId == participanteId)))
                .OrderByDescending(e => e.FechaInicio)
                .ToListAsync();
        }
    }
}