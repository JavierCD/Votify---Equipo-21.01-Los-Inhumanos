using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models;
using Votify.Services.Models;

namespace Votify.Core.Interfaces
{
    public interface IEventoService
    {
        Task<IEnumerable<Evento>> ObtenerTodosAsync();
        Task<Evento?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Evento>> ObtenerEventosPorOrganizadorAsync(int organizadorId);
        Task<Evento?> ObtenerEventoConDetallesAsync(int id);
        Task<Evento?> ObtenerEventoPorCodigoAsync(string codigo);
        Task<IEnumerable<Evento>> ObtenerEventosPorJuezAsync(int juezId);
        Task<Evento> CrearAsync(Evento evento);
        Task ActualizarAsync(EditarEventoRequest request);
        Task EliminarAsync(int id);
        Task AsignarJuezAEventoAsync(int juezId, int eventoId);
        Task DesasignarJuezAEventoAsync(int juezId, int eventoId);

        // TODO: Borrar cuando Login terminado
        Task<int> ObtenerOrganizadorMockIdAsync();

        Task CrearEventoDesdeFormularioAsync(CrearEventoRequest request);
    }
}
