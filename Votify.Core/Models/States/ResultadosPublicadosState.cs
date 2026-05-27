using Votify.Core.Enums;

namespace Votify.Core.Models
{
    public class ResultadosPublicadosState : IVotacionState
    {
        public override EstadoVotacion Tipo => EstadoVotacion.ResultadosPublicados;

        public override bool PuedeVotar(DateTime ahoraUtc, Votacion context)
        {
            return false;
        }
    }
}
