
using Votify.Core.Models;

namespace Votify.Core.Factories
{
    public abstract class EventoCreator
    {
        public abstract Evento CrearEvento(string name, DateTime fechaInicio, DateTime fechaFin, int organizadorId, string? descripcion = null);
    }
}