
namespace Votify.Core.Models
{
    public class HackathonEvent : Evento
    {

        protected HackathonEvent() { }
        public HackathonEvent(string name, DateTime fechaInicio, DateTime fechaFin, int organizadorId, string? descripcion = null)
            : base(name, fechaInicio, fechaFin, organizadorId, descripcion) { }

        public override string Modalidad() => "Hackathon";
    }
}