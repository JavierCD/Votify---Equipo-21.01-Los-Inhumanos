using Votify.Core.Models;

namespace Votify.Core.Factories
{
    public class HackathonEventCreator : EventoCreator
    {
        public override Evento CrearEvento(string name, DateTime fechaInicio, DateTime fechaFin, int organizadorId, string? descripcion = null)
        {
            return new HackathonEvent(name, fechaInicio, fechaFin, organizadorId, descripcion);
        }
    }
}