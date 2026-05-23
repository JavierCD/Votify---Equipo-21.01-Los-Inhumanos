using System;
using System.Linq;
using System.Threading.Tasks;
using Votify.Core.Models;

namespace Votify.Core.Models
{
    public class PuntuacionRangeHandler : AbstractVotoValidationHandler
    {
        public override async Task HandleAsync(VotoValidationContext context)
        {
            if (context.Votacion == null)
                throw new InvalidOperationException("No se pueden validar puntuaciones sin votación.");

            if (context.PuntuacionesPorProyecto == null || !context.PuntuacionesPorProyecto.Any())
                throw new ArgumentException("Debes puntuar al menos un proyecto.");

            var votacion = context.Votacion as Puntuacion;
            if (votacion == null)
                throw new InvalidOperationException("Esta validación solo aplica a votaciones de puntuación.");

            if (context.PuntuacionesPorProyecto.Any(p => p.Value < 0 || p.Value > votacion.ValorMax))
                throw new ArgumentException($"Las puntuaciones deben estar entre 0 y {votacion.ValorMax}.");

            if (context.PuntuacionesPorProyecto.Values.Sum() > votacion.ValorMax)
                throw new ArgumentException($"La suma total de puntuaciones no puede superar {votacion.ValorMax} puntos.");

            await HandleNextAsync(context);
        }
    }
}
