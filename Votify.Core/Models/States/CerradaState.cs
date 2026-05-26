namespace Votify.Core.Models
{
    public class CerradaState : IVotacionState
    {
        public override string Nombre => "Cerrada";

        public override void CerrarManual(Votacion context)
        {
            // Ya está cerrada, no hacer nada (idempotente)
        }

        public override void Pausar(Votacion context)
        {
            context.Estado = "Pausada";
            context.SetState(new PausadaState());
        }

        public override void Abrir(Votacion context)
        {
            context.Estado = "Abierta";
            context.EstaCerrada = false;
            context.FechaApertura = DateTime.UtcNow;
            if (context.FechaCierre <= context.FechaApertura)
                context.FechaCierre = context.FechaApertura.AddDays(1);
            context.SetState(new AbiertaState());
        }

        public override void PublicarResultados(Votacion context)
        {
            context.ResultadosPublicados = true;
            context.Estado = "ResultadosPublicados";
            context.SetState(new ResultadosPublicadosState());
        }

        public override bool PuedeVotar(DateTime ahoraUtc, Votacion context)
        {
            return false;
        }
    }
}
