namespace Votify.Core.Models
{
    public abstract class IVotacionState
    {
        public abstract string Nombre { get; }

        public virtual void Abrir(Votacion context)
        {
            throw new InvalidOperationException($"No se puede abrir una votación desde el estado '{Nombre}'.");
        }

        public virtual void Cerrar(Votacion context)
        {
            throw new InvalidOperationException($"No se puede cerrar una votación desde el estado '{Nombre}'.");
        }

        public virtual void CerrarManual(Votacion context)
        {
            throw new InvalidOperationException($"No se puede forzar el cierre desde el estado '{Nombre}'.");
        }

        public virtual void Pausar(Votacion context)
        {
            throw new InvalidOperationException($"No se puede pausar una votación desde el estado '{Nombre}'.");
        }

        public virtual void Reanudar(Votacion context)
        {
            throw new InvalidOperationException($"No se puede reanudar una votación desde el estado '{Nombre}'.");
        }

        public virtual void Programar(Votacion context)
        {
            throw new InvalidOperationException($"No se puede programar una votación desde el estado '{Nombre}'.");
        }

        public virtual void PublicarResultados(Votacion context)
        {
            throw new InvalidOperationException($"No se pueden publicar resultados desde el estado '{Nombre}'.");
        }

        public virtual void EvaluarTemporal(Votacion context, DateTime ahoraUtc)
        {
        }

        public virtual bool PuedeVotar(DateTime ahoraUtc, Votacion context)
        {
            return false;
        }
    }
}
