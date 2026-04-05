namespace Votify.Core.Models
{
    public class ESportsEvent : Evento
    {
        public ESportsEvent(string name, DateTime fechaInicio, DateTime fechaFin, int organizadorId, string? descripcion = null)
            : base(name, fechaInicio, fechaFin, organizadorId, descripcion) { }

        public override string Modalidad() => "E-Sports";
    }
}