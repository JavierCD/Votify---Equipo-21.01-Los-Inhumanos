namespace Votify.Core.Models
{
    public class CerradaManualState : IVotacionState
    {
        public override string Nombre => "CerradaManual";

        public override void Abrir(Votacion context)
        {
            context.Estado = "Abierta";
            context.EstaCerrada = false;
            context.FechaApertura = DateTime.UtcNow;
            if (context.FechaCierre <= context.FechaApertura)
                context.FechaCierre = context.FechaApertura.AddDays(1);
            context.SetState(new AbiertaState());
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

        public override void PublicarResultados(Votacion context)
        {
            context.ResultadosPublicados = true;
            context.Estado = "ResultadosPublicados";
            context.SetState(new ResultadosPublicadosState());
        }

        public override void CerrarManual(Votacion context)
        {
            // Ya está cerrada manualmente, no hacer nada (idempotente)
        }

        public override void Pausar(Votacion context)
        {
            context.Estado = "Pausada";
            context.SetState(new PausadaState());
        }

        public override void Cerrar(Votacion context)
        {
            context.EstaCerrada = true;
            context.Estado = "Cerrada";
            context.SetState(new CerradaState());
        }

        public override bool PuedeVotar(DateTime ahoraUtc, Votacion context)
        {
            return false;
        }
    }
}
