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
        
        Task<Participante?> ObtenerDashboardAsync(int id);
        Task AsignarProyectoACategoriaAsync(int proyectoId, int categoriaId);
        Task ReasignarProyectoACategoriaAsync(int proyectoId, int categoriaOrigenId, int categoriaDestinoId);
        Task ReasignarProyectoAOtroEventoAsync(int proyectoId, int eventoDestinoId, int categoriaDestinoId);
        Task<IEnumerable<Participante>> ObtenerParticipantesNoEnCategoriaAsync(int eventoId, int categoriaId);
        Task<IEnumerable<Proyecto>> ObtenerProyectosDisponiblesParaAsignarAsync(int participanteId, int eventoId, int categoriaId);
        Task<IEnumerable<Evento>> ObtenerEventosDisponiblesParaReasignarAsync(int eventoActualId);
        Task<IEnumerable<Categoria>> ObtenerCategoriasDeEventoAsync(int eventoId);


        // El futuro método para la IA (lo dejamos preparado)
        // Task<string> GenerarHojaDeRutaIAAsync(int id);
    }
}
