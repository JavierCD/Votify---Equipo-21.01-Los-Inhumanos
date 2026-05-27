using Moq;
using Votify.Core.Enums;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Implementations;
using Votify.Services.Models.Requests;
using Xunit;

namespace Votify.Tests.Services
{
    public class PopularServiceTests
    {
        private Mock<IUnitOfWork> CreateUnitOfWorkMock(bool categoriaExiste = true, bool yaExisteVotacion = false, Popular? votacion = null)
        {
            var mockRepo = new Mock<IPopularRepository>();
            mockRepo.Setup(r => r.CategoriaExisteAsync(It.IsAny<int>())).ReturnsAsync(categoriaExiste);
            mockRepo.Setup(r => r.YaExisteVotacionParaCategoriaAsync(It.IsAny<int>())).ReturnsAsync(yaExisteVotacion);
            mockRepo.Setup(r => r.ObtenerPorIdConCategoriaAsync(It.IsAny<int>())).ReturnsAsync(votacion);
            mockRepo.Setup(r => r.CrearAsync(It.IsAny<Popular>()))
                .ReturnsAsync((Popular p) => { p.Id = 1; return p; });

            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.PopularRepository).Returns(mockRepo.Object);

            return mockUoW;
        }

        [Fact]
        public async Task CrearVotacionPopularAsync_CuandoEsValido_CreaVotacion()
        {
            var mockUoW = CreateUnitOfWorkMock(categoriaExiste: true, yaExisteVotacion: false);
            var service = new PopularService(mockUoW.Object);

            var request = new CrearVotacionPopularRequest
            {
                CategoriaId = 1,
                FechaApertura = DateTime.UtcNow.AddDays(1),
                FechaCierre = DateTime.UtcNow.AddDays(2),
                Estado = EstadoVotacion.Programada,
                MaxSelection = 3
            };

            var resultado = await service.CrearVotacionPopularAsync(request);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            Assert.Equal(1, resultado.CategoriaId);
            Assert.Equal(3, resultado.MaxSelection);
            mockUoW.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CrearVotacionPopularAsync_CuandoRequestNull_LanzaArgumentNullException()
        {
            var mockUoW = CreateUnitOfWorkMock();
            var service = new PopularService(mockUoW.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => service.CrearVotacionPopularAsync(null!));
        }

        [Fact]
        public async Task CrearVotacionPopularAsync_CuandoCategoriaNoExiste_LanzaArgumentException()
        {
            var mockUoW = CreateUnitOfWorkMock(categoriaExiste: false);
            var service = new PopularService(mockUoW.Object);

            var request = new CrearVotacionPopularRequest
            {
                CategoriaId = 1,
                FechaApertura = DateTime.UtcNow.AddDays(1),
                FechaCierre = DateTime.UtcNow.AddDays(2),
                Estado = EstadoVotacion.Programada,
                MaxSelection = 3
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.CrearVotacionPopularAsync(request));
        }

        [Fact]
        public async Task CrearVotacionPopularAsync_CuandoFechaAperturaMayorQueCierre_LanzaArgumentException()
        {
            var mockUoW = CreateUnitOfWorkMock();
            var service = new PopularService(mockUoW.Object);

            var request = new CrearVotacionPopularRequest
            {
                CategoriaId = 1,
                FechaApertura = DateTime.UtcNow.AddDays(2),
                FechaCierre = DateTime.UtcNow.AddDays(1),
                Estado = EstadoVotacion.Programada,
                MaxSelection = 3
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.CrearVotacionPopularAsync(request));
        }

        [Fact]
        public async Task CrearVotacionPopularAsync_CuandoMaxSelectionMenorOIgualCero_LanzaArgumentException()
        {
            var mockUoW = CreateUnitOfWorkMock();
            var service = new PopularService(mockUoW.Object);

            var request = new CrearVotacionPopularRequest
            {
                CategoriaId = 1,
                FechaApertura = DateTime.UtcNow.AddDays(1),
                FechaCierre = DateTime.UtcNow.AddDays(2),
                Estado = EstadoVotacion.Programada,
                MaxSelection = 0
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.CrearVotacionPopularAsync(request));
        }

        [Fact]
        public async Task CrearVotacionPopularAsync_CuandoEstadoDefault_CreaConProgramada()
        {
            var mockUoW = CreateUnitOfWorkMock(categoriaExiste: true, yaExisteVotacion: false);
            var service = new PopularService(mockUoW.Object);

            var request = new CrearVotacionPopularRequest
            {
                CategoriaId = 1,
                FechaApertura = DateTime.UtcNow.AddDays(1),
                FechaCierre = DateTime.UtcNow.AddDays(2),
                Estado = default,
                MaxSelection = 3
            };

            var resultado = await service.CrearVotacionPopularAsync(request);

            Assert.NotNull(resultado);
            Assert.Equal(EstadoVotacion.Programada, resultado.Estado);
        }

        [Fact]
        public async Task CrearVotacionPopularAsync_CuandoYaExisteVotacion_LanzaInvalidOperationException()
        {
            var mockUoW = CreateUnitOfWorkMock(yaExisteVotacion: true);
            var service = new PopularService(mockUoW.Object);

            var request = new CrearVotacionPopularRequest
            {
                CategoriaId = 1,
                FechaApertura = DateTime.UtcNow.AddDays(1),
                FechaCierre = DateTime.UtcNow.AddDays(2),
                Estado = EstadoVotacion.Programada,
                MaxSelection = 3
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.CrearVotacionPopularAsync(request));
        }
    }
}
