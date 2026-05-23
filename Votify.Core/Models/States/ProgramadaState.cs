namespace Votify.Core.Models
{
    public class ProgramadaState : IVotacionState
    {
        public override string Nombre => "Programada";

        public override void Abrir(Votacion context)
        {
            context.Estado = "Abierta";
            context.EstaCerrada = false;
            context.FechaApertura = DateTime.UtcNow;
            if (context.FechaCierre <= context.FechaApertura)
                context.FechaCierre = context.FechaApertura.AddDays(1);
            context.SetState(new AbiertaState());
        }

        public override void Cerrar(Votacion context)
        {
            if (context.EstaCerrada)
                throw new InvalidOperationException("La votación ya está cerrada");
            context.EstaCerrada = true;
            context.Estado = "Cerrada";
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

        public override void EvaluarTemporal(Votacion context, DateTime ahoraUtc)
        {
            if (ahoraUtc >= context.FechaApertura && ahoraUtc <= context.FechaCierre)
            {
                context.Estado = "Abierta";
                context.SetState(new AbiertaState());
            }
            else if (ahoraUtc > context.FechaCierre)
            {
                context.Estado = "Cerrada";
                context.EstaCerrada = true;
                context.SetState(new CerradaState());
            }
        }

        public override bool PuedeVotar(DateTime ahoraUtc, Votacion context)
        {
            if (ahoraUtc >= context.FechaApertura && ahoraUtc <= context.FechaCierre)
            {
                return true;
            }
            return false;
        }
    }
}
