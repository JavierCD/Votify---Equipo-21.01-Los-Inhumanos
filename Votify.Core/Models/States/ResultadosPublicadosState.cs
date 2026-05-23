namespace Votify.Core.Models
{
    public class ResultadosPublicadosState : IVotacionState
    {
        public override string Nombre => "ResultadosPublicados";

        public override bool PuedeVotar(DateTime ahoraUtc, Votacion context)
        {
            return false;
        }
    }
}
