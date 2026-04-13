using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models;

namespace Votify.Services.Interfaces
{
    public interface INotificacionService
    {
        Task<List<Notificacion>> ObtenerNotificacionesUsuarioAsync(int miembroId);
        Task MarcarComoLeidaAsync(int notificacionId);
    }
}
