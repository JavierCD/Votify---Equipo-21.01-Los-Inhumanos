using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Core.Models;

namespace Votify.Core.Models
{
    public static class VotoValidationChainBuilder
    {
        public static IVotoValidationHandler BuildPuntuacionChain(
            Func<int, Task<Puntuacion?>> obtenerVotacion,
            Func<int, Task<List<Proyecto>>> obtenerProyectos,
            Func<int, string, Task<bool>> emailYaVoto)
        {
            var nonEmpty = new NonEmptySelectionHandler(
                ctx => ctx.PuntuacionesPorProyecto?.Count ?? 0,
                "Debes puntuar al menos un proyecto.");

            var range = new PuntuacionRangeHandler();

            var validos = new ProyectosValidosHandler(
                obtenerProyectos,
                ctx => ctx.PuntuacionesPorProyecto?.Keys ?? Enumerable.Empty<int>());

            var existe = new VotacionExistsHandler<Puntuacion>(obtenerVotacion);
            var abierta = new VotacionAbiertaHandler();
            var restriccion = new SingleVoteRestrictionHandler(emailYaVoto);

            existe.SetNext(abierta)
                  .SetNext(nonEmpty)
                  .SetNext(range)
                  .SetNext(validos)
                  .SetNext(restriccion);

            return existe;
        }

        public static IVotoValidationHandler BuildPopularChain(
            Func<int, Task<Popular?>> obtenerVotacion,
            Func<int, Task<List<Proyecto>>> obtenerProyectos,
            Func<int, string, Task<bool>> emailYaVoto)
        {
            var nonEmpty = new NonEmptySelectionHandler(
                ctx => ctx.ProyectosSeleccionadosIds?.Count ?? 0,
                "Debes seleccionar al menos un proyecto.");

            var maxSelection = new MaxSelectionHandler(
                ctx => ctx.ProyectosSeleccionadosIds?.Count ?? 0);

            var validos = new ProyectosValidosHandler(
                obtenerProyectos,
                ctx => ctx.ProyectosSeleccionadosIds ?? Enumerable.Empty<int>());

            var existe = new VotacionExistsHandler<Popular>(obtenerVotacion);
            var abierta = new VotacionAbiertaHandler();
            var restriccion = new SingleVoteRestrictionHandler(emailYaVoto);

            existe.SetNext(abierta)
                  .SetNext(nonEmpty)
                  .SetNext(maxSelection)
                  .SetNext(validos)
                  .SetNext(restriccion);

            return existe;
        }

        public static IVotoValidationHandler BuildMulticriterioChain(
            Func<int, Task<Multicriterio?>> obtenerVotacion,
            Func<int, string, Task<bool>> emailYaVoto)
        {
            var existe = new VotacionExistsHandler<Multicriterio>(obtenerVotacion);
            var abierta = new VotacionAbiertaHandler();
            var restriccion = new SingleVoteRestrictionHandler(emailYaVoto);

            existe.SetNext(abierta)
                  .SetNext(restriccion);

            return existe;
        }
    }
}
