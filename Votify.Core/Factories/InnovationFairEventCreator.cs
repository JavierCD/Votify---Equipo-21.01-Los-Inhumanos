using Votify.Core.Models;

namespace Votify.Core.Factories
{
    public class InnovationFairEventCreator : EventoCreator
    {
        public override Evento CrearEvento(string name, DateTime fechaInicio, DateTime fechaFin, int organizadorId, string? descripcion = null)
        {
            return new InnovationFairEvent(name, fechaInicio, fechaFin, organizadorId, descripcion);
        }
    }
}
