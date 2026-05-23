using Votify.Core.Models;
using Votify.Services.Implementations.Strategies;
using Xunit;

namespace Votify.Tests.Strategies
{
    public class MulticriterioRankingStrategyTests
    {
        private MulticriterioRankingStrategy CreateStrategy() => new MulticriterioRankingStrategy();

        [Fact]
        public void CalcularRanking_CalculaPuntuacionPonderada()
        {
            var strategy = CreateStrategy();
            var categoria = new Categoria { Id = 1, Name = "Test" };
            var proyecto1 = new AiProject("Proyecto A", 1) { Id = 1, FechaRegistro = DateTime.UtcNow.AddDays(-5) };
            var proyecto2 = new AiProject("Proyecto B", 1) { Id = 2, FechaRegistro = DateTime.UtcNow.AddDays(-3) };
            categoria.Proyectos = new List<Proyecto> { proyecto1, proyecto2 };

            var criterio1 = new Criterio { Id = 1, Name = "Calidad", Peso = 60 };
            var criterio2 = new Criterio { Id = 2, Name = "Innovacion", Peso = 40 };

            var votacion = new Multicriterio { Id = 1 };
            votacion.Criterios = new List<Criterio> { criterio1, criterio2 };
            votacion.Votos = new List<Voto>
            {
                new VotoExperto(1, proyecto1.Id, 0, false, null, null)
                {
                    Proyecto = proyecto1,
                    Detalles = new List<DetalleVoto>
                    {
                        new DetalleVoto { CriterioId = 1, Puntuacion = 8 },
                        new DetalleVoto { CriterioId = 2, Puntuacion = 9 }
                    }
                },
                new VotoExperto(2, proyecto2.Id, 0, false, null, null)
                {
                    Proyecto = proyecto2,
                    Detalles = new List<DetalleVoto>
                    {
                        new DetalleVoto { CriterioId = 1, Puntuacion = 7 },
                        new DetalleVoto { CriterioId = 2, Puntuacion = 6 }
                    }
                }
            };
            categoria.Votacion = votacion;

            var resultado = strategy.CalcularRanking(categoria);

            Assert.Equal(2, resultado.Count);
            Assert.Equal("Proyecto A", resultado[0].NombreProyecto);
            Assert.Equal("Proyecto B", resultado[1].NombreProyecto);
        }

        [Fact]
        public void CalcularRanking_SinCriterios_DevuelvePuntuacionCero()
        {
            var strategy = CreateStrategy();
            var categoria = new Categoria { Id = 1, Name = "Test" };
            var proyecto1 = new AiProject("Proyecto A", 1) { Id = 1, FechaRegistro = DateTime.UtcNow.AddDays(-5) };
            categoria.Proyectos = new List<Proyecto> { proyecto1 };

            var votacion = new Multicriterio { Id = 1 };
            votacion.Criterios = new List<Criterio>();
            votacion.Votos = new List<Voto>
            {
                new VotoExperto(1, proyecto1.Id, 0, false, null, null)
                {
                    Proyecto = proyecto1,
                    Detalles = new List<DetalleVoto>
                    {
                        new DetalleVoto { CriterioId = 1, Puntuacion = 8 }
                    }
                }
            };
            categoria.Votacion = votacion;

            var resultado = strategy.CalcularRanking(categoria);

            Assert.Single(resultado);
            Assert.Equal(0, resultado[0].PuntosTotales);
        }

        [Fact]
        public void CalcularRanking_SinVotos_DevuelveListaVacia()
        {
            var strategy = CreateStrategy();
            var categoria = new Categoria { Id = 1, Name = "Test" };
            categoria.Proyectos = new List<Proyecto>();

            var votacion = new Multicriterio { Id = 1 };
            votacion.Criterios = new List<Criterio>();
            votacion.Votos = new List<Voto>();
            categoria.Votacion = votacion;

            var resultado = strategy.CalcularRanking(categoria);

            Assert.Empty(resultado);
        }
    }
}
