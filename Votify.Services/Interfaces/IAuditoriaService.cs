using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Services.Models;

namespace Votify.Services.Interfaces
{
    public interface IAuditoriaService
    {
        Task <List<AuditoriaVotoResponse>> ObtenerHistorialPorEventoAsync(int eventoId);
    }
}
