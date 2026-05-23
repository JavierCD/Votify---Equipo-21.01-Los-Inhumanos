using System;
using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Core.Models;

namespace Votify.Core.Models
{
    public class VotacionExistsHandler<T> : AbstractVotoValidationHandler where T : Votacion
    {
        private readonly Func<int, Task<T?>> _obtenerVotacion;

        public VotacionExistsHandler(Func<int, Task<T?>> obtenerVotacion)
        {
            _obtenerVotacion = obtenerVotacion;
        }

        public override async Task HandleAsync(VotoValidationContext context)
        {
            var votacion = await _obtenerVotacion(context.VotacionId);
            if (votacion == null)
                throw new ArgumentException("La votación no existe.");

            context.Votacion = votacion;
            await HandleNextAsync(context);
        }
    }
}
