using Votify.Core.Enums;

namespace Votify.Core.Models
{
    public class CerradaState : IVotacionState
    {
        public override EstadoVotacion Tipo => EstadoVotacion.Cerrada;

        public override void Cerrar(Votacion context)
        {
            // Ya está cerrada, no hacer nada (idempotente)
        }

        public override void CerrarManual(Votacion context)
        {
            // Ya está cerrada, no hacer nada (idempotente)
        }

        public override void Pausar(Votacion context)
        {
            context.Estado = EstadoVotacion.Pausada;
            context.SetState(new PausadaState());
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

        public override void PublicarResultados(Votacion context)
        {
            context.ResultadosPublicados = true;
            context.Estado = EstadoVotacion.ResultadosPublicados;
            context.SetState(new ResultadosPublicadosState());
        }

        public override bool PuedeVotar(DateTime ahoraUtc, Votacion context)
        {
            return false;
        }
    }
}
