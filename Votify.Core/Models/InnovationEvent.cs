
namespace Votify.Core.Models
{
    public class InnovationFairEvent : Evento
    {

        protected InnovationFairEvent() { }
        public InnovationFairEvent(string name, DateTime fechaInicio, DateTime fechaFin, int organizadorId, string? descripcion = null)
            : base(name, fechaInicio, fechaFin, organizadorId, descripcion) { }

        public override string Modalidad() => "Feria de Innovación";
    }
}