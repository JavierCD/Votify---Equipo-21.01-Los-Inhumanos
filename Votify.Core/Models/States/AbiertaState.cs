namespace Votify.Core.Models
{
    public class AbiertaState : IVotacionState
    {
        public override string Nombre => "Abierta";

        public override void Cerrar(Votacion context)
        {
            if (context.EstaCerrada)
                throw new InvalidOperationException("La votación ya está cerrada");
            context.EstaCerrada = true;
            context.SetState(new CerradaState());
        }

        public override void CerrarManual(Votacion context)
        {
            context.Estado = "CerradaManual";
            context.EstaCerrada = true;
            context.FechaCierre = DateTime.UtcNow;
            context.SetState(new CerradaManualState());
        }

        public override void Pausar(Votacion context)
        {
            context.Estado = "Pausada";
            context.SetState(new PausadaState());
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

        public override void EvaluarTemporal(Votacion context, DateTime ahoraUtc)
        {
            if (ahoraUtc > context.FechaCierre)
            {
                context.Estado = "Cerrada";
                context.EstaCerrada = true;
                context.SetState(new CerradaState());
            }
        }

        public override bool PuedeVotar(DateTime ahoraUtc, Votacion context)
        {
            return ahoraUtc >= context.FechaApertura && ahoraUtc <= context.FechaCierre;
        }
    }
}
