using System;
using System.Threading.Tasks;
using Votify.Core.Models;

namespace Votify.Core.Models
{
    public class MaxSelectionHandler : AbstractVotoValidationHandler
    {
        private readonly Func<VotoValidationContext, int> _obtenerCantidad;

        public MaxSelectionHandler(Func<VotoValidationContext, int> obtenerCantidad)
        {
            _obtenerCantidad = obtenerCantidad;
        }

        public override async Task HandleAsync(VotoValidationContext context)
        {
            if (context.Votacion == null)
                throw new InvalidOperationException("No se puede validar selección máxima sin votación.");

            var votacion = context.Votacion as Popular;
            if (votacion == null)
                throw new InvalidOperationException("Esta validación solo aplica a votaciones populares.");

            int cantidad = _obtenerCantidad(context);
            if (cantidad > votacion.MaxSelection)
                throw new ArgumentException($"Solo puedes seleccionar hasta {votacion.MaxSelection} proyecto(s).");

            await HandleNextAsync(context);
        }
    }
}
