using Moq;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Implementations;
using Votify.Services.Models.Requests;
using Xunit;

namespace Votify.Tests.Services
{
    public class PuntuacionServiceTests
    {
        private Mock<IUnitOfWork> CreateUnitOfWorkMock(bool categoriaExiste = true, bool yaExisteVotacion = false)
        {
            var mockRepo = new Mock<IPuntuacionRepository>();
            mockRepo.Setup(r => r.CategoriaExisteAsync(It.IsAny<int>())).ReturnsAsync(categoriaExiste);
            mockRepo.Setup(r => r.YaExisteVotacionParaCategoriaAsync(It.IsAny<int>())).ReturnsAsync(yaExisteVotacion);
            mockRepo.Setup(r => r.CrearAsync(It.IsAny<Puntuacion>()))
                .ReturnsAsync((Puntuacion p) => { p.Id = 1; return p; });

            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.PuntuacionRepository).Returns(mockRepo.Object);

            return mockUoW;
        }

        [Fact]
        public async Task CrearVotacionPuntuacionAsync_CuandoEsValido_CreaVotacion()
        {
            var mockUoW = CreateUnitOfWorkMock(categoriaExiste: true, yaExisteVotacion: false);

            var service = new PuntuacionService(mockUoW.Object);

            var request = new CrearVotacionPuntuacionRequest
            {
                CategoriaId = 1,
                FechaApertura = DateTime.UtcNow.AddDays(1),
                FechaCierre = DateTime.UtcNow.AddDays(2),
                Estado = "Programada",
                ValorMax = 10
            };

            var resultado = await service.CrearVotacionPuntuacionAsync(request);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            Assert.Equal(1, resultado.CategoriaId);
            Assert.Equal(10, resultado.ValorMax);
            mockUoW.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CrearVotacionPuntuacionAsync_CuandoRequestNull_LanzaArgumentNullException()
        {
            var mockUoW = CreateUnitOfWorkMock();
            var service = new PuntuacionService(mockUoW.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => service.CrearVotacionPuntuacionAsync(null!));
        }

        [Fact]
        public async Task CrearVotacionPuntuacionAsync_CuandoCategoriaNoExiste_LanzaArgumentException()
        {
            var mockUoW = CreateUnitOfWorkMock(categoriaExiste: false);
            var service = new PuntuacionService(mockUoW.Object);

            var request = new CrearVotacionPuntuacionRequest
            {
                CategoriaId = 1,
                FechaApertura = DateTime.UtcNow.AddDays(1),
                FechaCierre = DateTime.UtcNow.AddDays(2),
                Estado = "Programada",
                ValorMax = 10
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.CrearVotacionPuntuacionAsync(request));
        }

        [Fact]
        public async Task CrearVotacionPuntuacionAsync_CuandoFechaAperturaMayorQueCierre_LanzaArgumentException()
        {
            var mockUoW = CreateUnitOfWorkMock();
            var service = new PuntuacionService(mockUoW.Object);

            var request = new CrearVotacionPuntuacionRequest
            {
                CategoriaId = 1,
                FechaApertura = DateTime.UtcNow.AddDays(2),
                FechaCierre = DateTime.UtcNow.AddDays(1),
                Estado = "Programada",
                ValorMax = 10
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.CrearVotacionPuntuacionAsync(request));
        }

        [Fact]
        public async Task CrearVotacionPuntuacionAsync_CuandoValorMaxMenorOIgualCero_LanzaArgumentException()
        {
            var mockUoW = CreateUnitOfWorkMock();
            var service = new PuntuacionService(mockUoW.Object);

            var request = new CrearVotacionPuntuacionRequest
            {
                CategoriaId = 1,
                FechaApertura = DateTime.UtcNow.AddDays(1),
                FechaCierre = DateTime.UtcNow.AddDays(2),
                Estado = "Programada",
                ValorMax = 0
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.CrearVotacionPuntuacionAsync(request));
        }

        [Fact]
        public async Task CrearVotacionPuntuacionAsync_CuandoEstadoVacio_LanzaArgumentException()
        {
            var mockUoW = CreateUnitOfWorkMock();
            var service = new PuntuacionService(mockUoW.Object);

            var request = new CrearVotacionPuntuacionRequest
            {
                CategoriaId = 1,
                FechaApertura = DateTime.UtcNow.AddDays(1),
                FechaCierre = DateTime.UtcNow.AddDays(2),
                Estado = "",
                ValorMax = 10
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.CrearVotacionPuntuacionAsync(request));
        }

        [Fact]
        public async Task CrearVotacionPuntuacionAsync_CuandoYaExisteVotacion_LanzaInvalidOperationException()
        {
            var mockUoW = CreateUnitOfWorkMock(yaExisteVotacion: true);
            var service = new PuntuacionService(mockUoW.Object);

            var request = new CrearVotacionPuntuacionRequest
            {
                CategoriaId = 1,
                FechaApertura = DateTime.UtcNow.AddDays(1),
                FechaCierre = DateTime.UtcNow.AddDays(2),
                Estado = "Programada",
                ValorMax = 10
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.CrearVotacionPuntuacionAsync(request));
        }
    }
}
