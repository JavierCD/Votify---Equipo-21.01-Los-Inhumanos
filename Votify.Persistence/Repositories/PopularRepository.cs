using Microsoft.EntityFrameworkCore;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Persistence.Context;
using Votify.Persistence.Entities;

namespace Votify.Persistence.Repositories
{
    public class PopularRepository : IPopularRepository
    {
        private readonly VotifyContext _context;

        public PopularRepository(VotifyContext context)
        {
            _context = context;
        }

        public async Task<bool> EventoExisteAsync(int eventoId)
        {
            return await _context.Eventos.AnyAsync(e => e.Id == eventoId);
        }

        public async Task<Popular> CrearAsync(Popular popular)
        {
            var entity = new PopularEntity
            {
                EventoId = popular.EventoId,
                FechaApertura = popular.FechaApertura,
                FechaCierre = popular.FechaCierre,
                Estado = popular.Estado,
                MaxSelecciones = popular.MaxSelecciones
            };

            _context.Votaciones.Add(entity);
            await _context.SaveChangesAsync();

            return new Popular
            {
                Id = entity.Id,
                EventoId = entity.EventoId,
                FechaApertura = entity.FechaApertura,
                FechaCierre = entity.FechaCierre,
                Estado = entity.Estado,
                MaxSelecciones = entity.MaxSelecciones
            };
        }
    }
}