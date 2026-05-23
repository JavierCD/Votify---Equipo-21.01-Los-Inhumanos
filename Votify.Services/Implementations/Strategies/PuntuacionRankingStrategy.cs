using System.Collections.Generic;
using System.Linq;
using Votify.Services.Interfaces;
using Votify.Core.Models;
using Votify.Services.Models.Responses;

namespace Votify.Services.Implementations.Strategies
{
    public class PuntuacionRankingStrategy : IRankingStrategy
    {
        public List<PosicionRankingResponse> CalcularRanking(Categoria categoria)
        {
            return categoria.Votacion.Votos
                .Where(v => v.Proyecto != null)
                .GroupBy(v => v.Proyecto)
                .Select(g => new PosicionRankingResponse
                {
                    NombreProyecto = g.Key!.Name,
                    PuntosTotales = g.Sum(v => v.PuntuacionBase),
                    FechaInscripcion = g.Key.FechaRegistro
                })
                .OrderByDescending(x => x.PuntosTotales)
                .ThenBy(x => x.FechaInscripcion)
                .ToList();
        }
    }
}
