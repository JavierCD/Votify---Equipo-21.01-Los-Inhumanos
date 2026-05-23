using System.Collections.Generic;
using System.Linq;
using Votify.Services.Interfaces;
using Votify.Core.Models;
using Votify.Services.Models.Responses;

namespace Votify.Services.Implementations.Strategies
{
    public class MulticriterioRankingStrategy : IRankingStrategy
    {
        public List<PosicionRankingResponse> CalcularRanking(Categoria categoria)
        {
            var mc = categoria.Votacion as Multicriterio;
            if (mc == null) return new List<PosicionRankingResponse>();

            return categoria.Proyectos
                .Select(p =>
                {
                    var votosProyecto = categoria.Votacion.Votos
                        .Where(v => v.ProyectoId == p.Id)
                        .ToList();
                    double puntaje = CalcularPuntuacionMulticriterio(votosProyecto, mc);
                    return new PosicionRankingResponse
                    {
                        NombreProyecto = p.Name,
                        PuntosTotales = puntaje,
                        FechaInscripcion = p.FechaRegistro
                    };
                })
                .OrderByDescending(x => x.PuntosTotales)
                .ThenBy(x => x.FechaInscripcion)
                .ToList();
        }

        private double CalcularPuntuacionMulticriterio(List<Voto> votos, Multicriterio votacion)
        {
            if (votacion.Criterios == null || !votacion.Criterios.Any())
                return 0;

            double sumaPonderada = 0;
            int votosValidos = 0;

            foreach (var voto in votos)
            {
                if (!voto.Detalles.Any()) continue;

                double puntajeVoto = 0;
                double pesoTotalUsado = 0;

                foreach (var detalle in voto.Detalles)
                {
                    var criterio = votacion.Criterios.FirstOrDefault(c => c.Id == detalle.CriterioId);
                    if (criterio != null && criterio.Peso > 0)
                    {
                        puntajeVoto += detalle.Puntuacion * (criterio.Peso / 100.0);
                        pesoTotalUsado += criterio.Peso;
                    }
                }

                if (pesoTotalUsado > 0 && pesoTotalUsado < 100)
                {
                    puntajeVoto = puntajeVoto * (100.0 / pesoTotalUsado);
                }

                sumaPonderada += puntajeVoto;
                votosValidos++;
            }

            return votosValidos > 0 ? sumaPonderada / votosValidos : 0;
        }
    }
}
