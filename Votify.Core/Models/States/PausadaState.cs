using Votify.Core.Enums;

namespace Votify.Core.Models
{
    public class PausadaState : IVotacionState
    {
        public override EstadoVotacion Tipo => EstadoVotacion.Pausada;

        public override void Reanudar(Votacion context)
        {
            context.EvaluarEstadoTemporal(DateTime.UtcNow);
        }

        public override void CerrarManual(Votacion context)
        {
            context.Estado = EstadoVotacion.Cerrada;
            context.EstaCerrada = true;
            context.FechaCierre = DateTime.UtcNow;
            context.SetState(new CerradaState());
        }

        public override void Programar(Votacion context)
        {
            context.Estado = EstadoVotacion.Programada;
            context.EstaCerrada = false;
            if (context.FechaApertura <= DateTime.UtcNow)
                context.FechaApertura = DateTime.UtcNow.AddDays(1);
            if (context.FechaCierre <= context.FechaApertura)
                context.FechaCierre = context.FechaApertura.AddDays(1);
            context.SetState(new ProgramadaState());
        }

        public override void Pausar(Votacion context)
        {
            // Ya está pausada, no hacer nada (idempotente)
        }

        public override void Abrir(Votacion context)
        {
            context.Estado = EstadoVotacion.Abierta;
            context.EstaCerrada = false;
            context.FechaApertura = DateTime.UtcNow;
            if (context.FechaCierre <= context.FechaApertura)
                context.FechaCierre = context.FechaApertura.AddDays(1);
            context.SetState(new AbiertaState());
        }

        public override void Cerrar(Votacion context)
        {
            context.EstaCerrada = true;
            context.Estado = EstadoVotacion.Cerrada;
            context.SetState(new CerradaState());
        }

        public override bool PuedeVotar(DateTime ahoraUtc, Votacion context)
        {
            return false;
        }
    }
}
