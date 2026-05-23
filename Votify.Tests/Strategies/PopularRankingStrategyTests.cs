using Votify.Core.Models;
using Votify.Services.Implementations.Strategies;
using Xunit;

namespace Votify.Tests.Strategies
{
    public class PopularRankingStrategyTests
    {
        private PopularRankingStrategy CreateStrategy() => new PopularRankingStrategy();

        [Fact]
        public void CalcularRanking_OrdenaPorCantidadDeVotos()
        {
            var strategy = CreateStrategy();
            var categoria = new Categoria { Id = 1, Name = "Test" };
            var proyecto1 = new AiProject("Proyecto A", 1) { Id = 1, FechaRegistro = DateTime.UtcNow.AddDays(-5) };
            var proyecto2 = new AiProject("Proyecto B", 1) { Id = 2, FechaRegistro = DateTime.UtcNow.AddDays(-3) };
            categoria.Proyectos = new List<Proyecto> { proyecto1, proyecto2 };

            var votacion = new Popular { Id = 1 };
            votacion.Votos = new List<Voto>
            {
                new VotoPublico(1, proyecto1.Id, 1, false, null, null) { Proyecto = proyecto1 },
                new VotoPublico(2, proyecto1.Id, 1, false, null, null) { Proyecto = proyecto1 },
                new VotoPublico(3, proyecto1.Id, 1, false, null, null) { Proyecto = proyecto1 },
                new VotoPublico(4, proyecto2.Id, 1, false, null, null) { Proyecto = proyecto2 },
                new VotoPublico(5, proyecto2.Id, 1, false, null, null) { Proyecto = proyecto2 }
            };
            categoria.Votacion = votacion;

            var resultado = strategy.CalcularRanking(categoria);

            Assert.Equal(2, resultado.Count);
            Assert.Equal("Proyecto A", resultado[0].NombreProyecto);
            Assert.Equal(3, resultado[0].PuntosTotales);
            Assert.Equal("Proyecto B", resultado[1].NombreProyecto);
            Assert.Equal(2, resultado[1].PuntosTotales);
        }

        [Fact]
        public void CalcularRanking_OrdenaPorFechaInscripcionEnCasoDeEmpate()
        {
            var strategy = CreateStrategy();
            var categoria = new Categoria { Id = 1, Name = "Test" };
            var proyecto1 = new AiProject("Proyecto A", 1) { Id = 1, FechaRegistro = DateTime.UtcNow.AddDays(-5) };
            var proyecto2 = new AiProject("Proyecto B", 1) { Id = 2, FechaRegistro = DateTime.UtcNow.AddDays(-3) };
            categoria.Proyectos = new List<Proyecto> { proyecto1, proyecto2 };

            var votacion = new Popular { Id = 1 };
            votacion.Votos = new List<Voto>
            {
                new VotoPublico(1, proyecto1.Id, 1, false, null, null) { Proyecto = proyecto1 },
                new VotoPublico(2, proyecto2.Id, 1, false, null, null) { Proyecto = proyecto2 }
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

            var votacion = new Popular { Id = 1 };
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

            var votacion = new Popular { Id = 1 };
            votacion.Votos = new List<Voto>
            {
                new VotoPublico(1, proyecto1.Id, 1, false, null, null) { Proyecto = proyecto1 },
                new VotoPublico(2, 999, 1, false, null, null) { Proyecto = null }
            };
            categoria.Votacion = votacion;

            var resultado = strategy.CalcularRanking(categoria);

            Assert.Single(resultado);
            Assert.Equal("Proyecto A", resultado[0].NombreProyecto);
        }
    }
}
