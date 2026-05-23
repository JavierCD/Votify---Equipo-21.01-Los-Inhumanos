using Votify.Core.Models;
using Votify.Services.Implementations.Strategies;
using Xunit;

namespace Votify.Tests.Strategies
{
    public class PuntuacionRankingStrategyTests
    {
        private PuntuacionRankingStrategy CreateStrategy() => new PuntuacionRankingStrategy();

        [Fact]
        public void CalcularRanking_OrdenaPorSumaDePuntuaciones()
        {
            var strategy = CreateStrategy();
            var categoria = new Categoria { Id = 1, Name = "Test" };
            var proyecto1 = new AiProject("Proyecto A", 1) { Id = 1, FechaRegistro = DateTime.UtcNow.AddDays(-5) };
            var proyecto2 = new AiProject("Proyecto B", 1) { Id = 2, FechaRegistro = DateTime.UtcNow.AddDays(-3) };
            categoria.Proyectos = new List<Proyecto> { proyecto1, proyecto2 };

            var votacion = new Puntuacion { Id = 1 };
            votacion.Votos = new List<Voto>
            {
                new VotoExperto(1, proyecto1.Id, 8, false, null, null) { Proyecto = proyecto1, PuntuacionBase = 8 },
                new VotoExperto(2, proyecto1.Id, 9, false, null, null) { Proyecto = proyecto1, PuntuacionBase = 9 },
                new VotoExperto(3, proyecto2.Id, 7, false, null, null) { Proyecto = proyecto2, PuntuacionBase = 7 },
                new VotoExperto(4, proyecto2.Id, 6, false, null, null) { Proyecto = proyecto2, PuntuacionBase = 6 }
            };
            categoria.Votacion = votacion;

            var resultado = strategy.CalcularRanking(categoria);

            Assert.Equal(2, resultado.Count);
            Assert.Equal("Proyecto A", resultado[0].NombreProyecto);
            Assert.Equal(17, resultado[0].PuntosTotales);
            Assert.Equal("Proyecto B", resultado[1].NombreProyecto);
            Assert.Equal(13, resultado[1].PuntosTotales);
        }

        [Fact]
        public void CalcularRanking_OrdenaPorFechaInscripcionEnCasoDeEmpate()
        {
            var strategy = CreateStrategy();
            var categoria = new Categoria { Id = 1, Name = "Test" };
            var proyecto1 = new AiProject("Proyecto A", 1) { Id = 1, FechaRegistro = DateTime.UtcNow.AddDays(-5) };
            var proyecto2 = new AiProject("Proyecto B", 1) { Id = 2, FechaRegistro = DateTime.UtcNow.AddDays(-3) };
            categoria.Proyectos = new List<Proyecto> { proyecto1, proyecto2 };

            var votacion = new Puntuacion { Id = 1 };
            votacion.Votos = new List<Voto>
            {
                new VotoExperto(1, proyecto1.Id, 8, false, null, null) { Proyecto = proyecto1, PuntuacionBase = 8 },
                new VotoExperto(2, proyecto2.Id, 8, false, null, null) { Proyecto = proyecto2, PuntuacionBase = 8 }
            };
            categoria.Votacion = votacion;

            var resultado = strategy.CalcularRanking(categoria);

            Assert.Equal(2, resultado.Count);
            Assert.Equal("Proyecto A", resultado[0].NombreProyecto);
            Assert.Equal("Proyecto B", resultado[1].NombreProyecto);
        }

        [Fact]
        public void CalcularRanking_SinVotos_DevuelveListaVacia()
        {
            var strategy = CreateStrategy();
            var categoria = new Categoria { Id = 1, Name = "Test" };
            categoria.Proyectos = new List<Proyecto>();

            var votacion = new Puntuacion { Id = 1 };
            votacion.Votos = new List<Voto>();
            categoria.Votacion = votacion;

            var resultado = strategy.CalcularRanking(categoria);

            Assert.Empty(resultado);
        }

        [Fact]
        public void CalcularRanking_ExcluyeVotosSinProyecto()
        {
            var strategy = CreateStrategy();
            var categoria = new Categoria { Id = 1, Name = "Test" };
            var proyecto1 = new AiProject("Proyecto A", 1) { Id = 1, FechaRegistro = DateTime.UtcNow.AddDays(-5) };
            categoria.Proyectos = new List<Proyecto> { proyecto1 };

            var votacion = new Puntuacion { Id = 1 };
            votacion.Votos = new List<Voto>
            {
                new VotoExperto(1, proyecto1.Id, 8, false, null, null) { Proyecto = proyecto1, PuntuacionBase = 8 },
                new VotoExperto(2, 999, 7, false, null, null) { Proyecto = null, PuntuacionBase = 7 }
            };
            categoria.Votacion = votacion;

            var resultado = strategy.CalcularRanking(categoria);

            Assert.Single(resultado);
            Assert.Equal("Proyecto A", resultado[0].NombreProyecto);
        }
    }
}
