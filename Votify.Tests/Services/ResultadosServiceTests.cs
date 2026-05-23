using Moq;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Implementations;
using Votify.Services.Implementations.Strategies;
using Votify.Services.Interfaces;
using Votify.Services.Models.Requests;
using Xunit;

namespace Votify.Tests.Services
{
    public class ResultadosServiceTests
    {
        private Mock<IUnitOfWork> CreateUnitOfWorkMock(Categoria? categoria = null, List<ResultadoIntervenido>? intervenidos = null)
        {
            var mockCategoriaRepo = new Mock<ICategoriaRepository>();
            mockCategoriaRepo.Setup(r => r.ObtenerCategoriaConVotacionYVotosAsync(It.IsAny<int>())).ReturnsAsync(categoria);

            var mockEventoRepo = new Mock<IEventoRepository>();

            var mockResultadosIntervenidos = new Mock<IGenericRepository<ResultadoIntervenido>>();
            mockResultadosIntervenidos.Setup(r => r.GetAllWithIncludesAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ResultadoIntervenido, object>>>()))
                .ReturnsAsync(intervenidos ?? new List<ResultadoIntervenido>());
            mockResultadosIntervenidos.Setup(r => r.GetAllAsync()).ReturnsAsync(intervenidos ?? new List<ResultadoIntervenido>());

            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.CategoriaRepository).Returns(mockCategoriaRepo.Object);
            mockUoW.Setup(u => u.EventoRepository).Returns(mockEventoRepo.Object);
            mockUoW.Setup(u => u.ResultadosIntervenidos).Returns(mockResultadosIntervenidos.Object);

            return mockUoW;
        }

        private Mock<IEmailService> CreateEmailServiceMock() => new Mock<IEmailService>();
        private Mock<IEmailTemplateBuilder> CreateTemplateBuilderMock() => new Mock<IEmailTemplateBuilder>();

        [Fact]
        public async Task CompartirClasificacionAsync_CuandoCategoriaNoExiste_LanzaExcepcion()
        {
            var mockUoW = CreateUnitOfWorkMock(categoria: null);
            var mockEmail = CreateEmailServiceMock();
            var mockTemplate = CreateTemplateBuilderMock();
            var mockFactory = new Mock<RankingStrategyFactory>(
                new Mock<MulticriterioRankingStrategy>().Object,
                new Mock<PopularRankingStrategy>().Object,
                new Mock<PuntuacionRankingStrategy>().Object
            );

            var service = new ResultadosService(mockUoW.Object, mockEmail.Object, mockTemplate.Object, mockFactory.Object);

            await Assert.ThrowsAsync<Exception>(() => service.CompartirClasificacionAsync(999));
        }

        [Fact]
        public async Task CompartirClasificacionAsync_CuandoVotacionNoExiste_LanzaExcepcion()
        {
            var categoria = new Categoria { Id = 1, Name = "Test" };
            var mockUoW = CreateUnitOfWorkMock(categoria: categoria);
            var mockEmail = CreateEmailServiceMock();
            var mockTemplate = CreateTemplateBuilderMock();
            var mockFactory = new Mock<RankingStrategyFactory>(
                new Mock<MulticriterioRankingStrategy>().Object,
                new Mock<PopularRankingStrategy>().Object,
                new Mock<PuntuacionRankingStrategy>().Object
            );

            var service = new ResultadosService(mockUoW.Object, mockEmail.Object, mockTemplate.Object, mockFactory.Object);

            await Assert.ThrowsAsync<Exception>(() => service.CompartirClasificacionAsync(1));
        }

        [Fact]
        public async Task GuardarResultadosIntervenidosAsync_BorraExistentesYAgregaNuevos()
        {
            var existentes = new List<ResultadoIntervenido>
            {
                new ResultadoIntervenido { Id = 1, VotacionId = 1, ProyectoId = 1, Posicion = 1 }
            };

            var mockUoW = CreateUnitOfWorkMock(intervenidos: existentes);
            var mockEmail = CreateEmailServiceMock();
            var mockTemplate = CreateTemplateBuilderMock();
            var mockFactory = new Mock<RankingStrategyFactory>(
                new Mock<MulticriterioRankingStrategy>().Object,
                new Mock<PopularRankingStrategy>().Object,
                new Mock<PuntuacionRankingStrategy>().Object
            );

            var service = new ResultadosService(mockUoW.Object, mockEmail.Object, mockTemplate.Object, mockFactory.Object);

            var nuevosResultados = new List<GuardarResultadoRequest>
            {
                new GuardarResultadoRequest { ProyectoId = 2, Posicion = 1, PuntajeOriginal = 10 }
            };

            await service.GuardarResultadosIntervenidosAsync(1, nuevosResultados);

            mockUoW.Verify(u => u.ResultadosIntervenidos.DeleteAsync(1), Times.Once);
            mockUoW.Verify(u => u.ResultadosIntervenidos.AddAsync(It.Is<ResultadoIntervenido>(r => r.ProyectoId == 2)), Times.Once);
            mockUoW.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GuardarResultadosIntervenidosAsync_CuandoNoHayExistentes_SoloAgrega()
        {
            var mockUoW = CreateUnitOfWorkMock(intervenidos: new List<ResultadoIntervenido>());
            var mockEmail = CreateEmailServiceMock();
            var mockTemplate = CreateTemplateBuilderMock();
            var mockFactory = new Mock<RankingStrategyFactory>(
                new Mock<MulticriterioRankingStrategy>().Object,
                new Mock<PopularRankingStrategy>().Object,
                new Mock<PuntuacionRankingStrategy>().Object
            );

            var service = new ResultadosService(mockUoW.Object, mockEmail.Object, mockTemplate.Object, mockFactory.Object);

            var nuevosResultados = new List<GuardarResultadoRequest>
            {
                new GuardarResultadoRequest { ProyectoId = 1, Posicion = 1, PuntajeOriginal = 10 }
            };

            await service.GuardarResultadosIntervenidosAsync(1, nuevosResultados);

            mockUoW.Verify(u => u.ResultadosIntervenidos.DeleteAsync(It.IsAny<int>()), Times.Never);
            mockUoW.Verify(u => u.ResultadosIntervenidos.AddAsync(It.Is<ResultadoIntervenido>(r => r.ProyectoId == 1)), Times.Once);
            mockUoW.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
