using System;
using System.Threading.Tasks;
using Votify.Core.Models;

namespace Votify.Core.Models
{
    public class NonEmptySelectionHandler : AbstractVotoValidationHandler
    {
        private readonly Func<VotoValidationContext, int> _obtenerCantidad;
        private readonly string _mensajeVacio;

        public NonEmptySelectionHandler(Func<VotoValidationContext, int> obtenerCantidad, string mensajeVacio)
        {
            _obtenerCantidad = obtenerCantidad;
            _mensajeVacio = mensajeVacio;
        }

        public override async Task HandleAsync(VotoValidationContext context)
        {
            if (_obtenerCantidad(context) <= 0)
                throw new ArgumentException(_mensajeVacio);

            await HandleNextAsync(context);
        }
    }
}
