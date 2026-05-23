using Moq;
using Votify.Core.Models;
using Votify.Services.Implementations.Strategies;
using Votify.Services.Interfaces;
using Xunit;

namespace Votify.Tests.Strategies
{
    public class RankingStrategyFactoryTests
    {
        [Fact]
        public void GetStrategy_ParaMulticriterio_DevuelveMulticriterioRankingStrategy()
        {
            var multicriterioStrategy = new MulticriterioRankingStrategy();
            var popularStrategy = new Mock<PopularRankingStrategy>().Object;
            var puntuacionStrategy = new Mock<PuntuacionRankingStrategy>().Object;

            var factory = new RankingStrategyFactory(multicriterioStrategy, popularStrategy, puntuacionStrategy);

            var votacion = new Multicriterio { Id = 1 };

            var resultado = factory.GetStrategy(votacion);

            Assert.IsType<MulticriterioRankingStrategy>(resultado);
        }

        [Fact]
        public void GetStrategy_ParaPopular_DevuelvePopularRankingStrategy()
        {
            var multicriterioStrategy = new Mock<MulticriterioRankingStrategy>().Object;
            var popularStrategy = new PopularRankingStrategy();
            var puntuacionStrategy = new Mock<PuntuacionRankingStrategy>().Object;

            var factory = new RankingStrategyFactory(multicriterioStrategy, popularStrategy, puntuacionStrategy);

            var votacion = new Popular { Id = 1 };

            var resultado = factory.GetStrategy(votacion);

            Assert.IsType<PopularRankingStrategy>(resultado);
        }

        [Fact]
        public void GetStrategy_ParaPuntuacion_DevuelvePuntuacionRankingStrategy()
        {
            var multicriterioStrategy = new Mock<MulticriterioRankingStrategy>().Object;
            var popularStrategy = new Mock<PopularRankingStrategy>().Object;
            var puntuacionStrategy = new PuntuacionRankingStrategy();

            var factory = new RankingStrategyFactory(multicriterioStrategy, popularStrategy, puntuacionStrategy);

            var votacion = new Puntuacion { Id = 1 };

            var resultado = factory.GetStrategy(votacion);

            Assert.IsType<PuntuacionRankingStrategy>(resultado);
        }

        [Fact]
        public void GetStrategy_ParaTipoDesconocido_LanzaExcepcion()
        {
            var multicriterioStrategy = new Mock<MulticriterioRankingStrategy>().Object;
            var popularStrategy = new Mock<PopularRankingStrategy>().Object;
            var puntuacionStrategy = new Mock<PuntuacionRankingStrategy>().Object;

            var factory = new RankingStrategyFactory(multicriterioStrategy, popularStrategy, puntuacionStrategy);

            var votacion = new VotacionDesconocida { Id = 1 };

            Assert.Throws<InvalidOperationException>(() => factory.GetStrategy(votacion));
        }

        private class VotacionDesconocida : Votacion { }
    }
}
