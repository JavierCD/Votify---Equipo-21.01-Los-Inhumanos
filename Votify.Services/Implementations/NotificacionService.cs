using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Interfaces;

namespace Votify.Services.Implementations
{
    public class NotificacionService : INotificacionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificacionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Notificacion>> ObtenerNotificacionesUsuarioAsync(int miembroId)
        {
            var notificaciones = await _unitOfWork.Notificaciones.GetAllAsync();
            return notificaciones
                .Where(n => n.MiembroId == miembroId && !n.Leida)
                .OrderByDescending(n => n.FechaCreacion)
                .ToList();
        }

        public async Task MarcarComoLeidaAsync(int notificacionId)
        {
            var notificacion = await _unitOfWork.Notificaciones.GetByIdAsync(notificacionId);

            if (notificacion != null && !notificacion.Leida)
            {
                notificacion.MarcarComoLeida();
                await _unitOfWork.Notificaciones.UpdateAsync(notificacion);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}