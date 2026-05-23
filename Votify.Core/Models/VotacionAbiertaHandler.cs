using System;
using System.Threading.Tasks;
using Votify.Core.Models;

namespace Votify.Core.Models
{
    public class VotacionAbiertaHandler : AbstractVotoValidationHandler
    {
        public override async Task HandleAsync(VotoValidationContext context)
        {
            if (context.Votacion == null)
                throw new InvalidOperationException("No se puede validar el estado de una votación nula.");

            if (!context.Votacion.PuedeVotar(DateTime.UtcNow))
                throw new InvalidOperationException("La votación no está abierta en este momento.");

            await HandleNextAsync(context);
        }
    }
}
