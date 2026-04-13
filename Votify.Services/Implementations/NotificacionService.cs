using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Votify.Core.Models;
using Votify.Persistence.Context;
using Votify.Services.Interfaces;

namespace Votify.Services.Implementations
{
    public class NotificacionService : INotificacionService
    {
        private readonly VotifyContext _context;

        public NotificacionService(VotifyContext context)
        {
            _context = context;
        }

        public async Task<List<Notificacion>> ObtenerNotificacionesUsuarioAsync(int miembroId)
        {
            return await _context.Set<Notificacion>()
                .AsNoTracking()
                .Where(n => n.MiembroId == miembroId && !n.Leida)
                .OrderByDescending(n => n.FechaCreacion)
                .ToListAsync();
        }

        public async Task MarcarComoLeidaAsync(int notificacionId)
        {
            // Buscamos la notificación
            var notificacion = await _context.Set<Notificacion>().FindAsync(notificacionId);

            if (notificacion != null && !notificacion.Leida)
            {
                // La marcamos como leída
                notificacion.MarcarComoLeida();
                await _context.SaveChangesAsync();
            }
        }
    }
}