using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Votify.Core.Models;

namespace Votify.Core.Models
{
    public class ProyectosValidosHandler : AbstractVotoValidationHandler
    {
        private readonly Func<int, Task<List<Proyecto>>> _obtenerProyectos;
        private readonly Func<VotoValidationContext, IEnumerable<int>> _obtenerProyectoIds;

        public ProyectosValidosHandler(
            Func<int, Task<List<Proyecto>>> obtenerProyectos,
            Func<VotoValidationContext, IEnumerable<int>> obtenerProyectoIds)
        {
            _obtenerProyectos = obtenerProyectos;
            _obtenerProyectoIds = obtenerProyectoIds;
        }

        public override async Task HandleAsync(VotoValidationContext context)
        {
            if (context.Votacion == null)
                throw new InvalidOperationException("No se pueden validar proyectos sin votación.");

            var proyectosValidos = await _obtenerProyectos(context.Votacion.CategoriaId);
            var proyectosValidosIds = proyectosValidos.Select(p => p.Id).ToHashSet();
            context.ProyectosValidos = proyectosValidos;

            var proyectoIds = _obtenerProyectoIds(context).ToList();
            if (proyectoIds.Any(id => !proyectosValidosIds.Contains(id)))
                throw new ArgumentException("Uno o más proyectos no pertenecen a la categoría de la votación.");

            await HandleNextAsync(context);
        }
    }
}
