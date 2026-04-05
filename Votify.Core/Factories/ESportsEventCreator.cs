using Votify.Core.Models;

namespace Votify.Core.Factories
{
    public class ESportsEventCreator : EventoCreator
    {
        public override Evento CrearEvento(string name, DateTime fechaInicio, DateTime fechaFin, int organizadorId, string? descripcion = null)
        {
            return new ESportsEvent(name, fechaInicio, fechaFin, organizadorId, descripcion);
        }
    }
}