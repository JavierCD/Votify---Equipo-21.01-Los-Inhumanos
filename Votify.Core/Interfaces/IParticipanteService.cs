using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Votify.Core.Models;

namespace Votify.Core.Interfaces
{
    public interface IParticipanteService
    {
        Task<Participante?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Participante>> ObtenerTodosAsync();

        // Métodos específicos de la Ficha del Participante
        Task ActualizarFichaAsync(Participante participante);
        Task CambiarEstadoAsync(int id, string nuevoEstado);
        Task CambiarVisibilidadAsync(int id, bool visible);
        Task<Participante?> ObtenerDashboardAsync(int id);


        // El futuro método para la IA (lo dejamos preparado)
        // Task<string> GenerarHojaDeRutaIAAsync(int id);
    }
}
