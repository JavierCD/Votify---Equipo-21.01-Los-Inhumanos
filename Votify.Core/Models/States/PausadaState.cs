namespace Votify.Core.Models
{
    public class PausadaState : IVotacionState
    {
        public override string Nombre => "Pausada";

        public override void Reanudar(Votacion context)
        {
            context.EvaluarEstadoTemporal(DateTime.UtcNow);
        }

        public override void CerrarManual(Votacion context)
        {
            context.Estado = "CerradaManual";
            context.EstaCerrada = true;
            context.FechaCierre = DateTime.UtcNow;
            context.SetState(new CerradaManualState());
        }

        public override void Programar(Votacion context)
        {
            context.Estado = "Programada";
            context.EstaCerrada = false;
            if (context.FechaApertura <= DateTime.UtcNow)
                context.FechaApertura = DateTime.UtcNow.AddDays(1);
            if (context.FechaCierre <= context.FechaApertura)
                context.FechaCierre = context.FechaApertura.AddDays(1);
            context.SetState(new ProgramadaState());
        }

        public override bool PuedeVotar(DateTime ahoraUtc, Votacion context)
        {
            return false;
        }
    }
}
