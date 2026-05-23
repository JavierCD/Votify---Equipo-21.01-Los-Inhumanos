using Moq;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Implementations;
using Votify.Services.Models.Requests;
using Xunit;

namespace Votify.Tests.Services
{
    public class EventoServiceTests
    {
        private Mock<IUnitOfWork> CreateUnitOfWorkMock(Evento? evento = null)
        {
            var mockEventos = new Mock<IGenericRepository<Evento>>();
            mockEventos.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(evento);

            var mockEventoRepo = new Mock<IEventoRepository>();
            mockEventoRepo.Setup(r => r.ObtenerEventoConDetallesAsync(It.IsAny<int>())).ReturnsAsync(evento);
            mockEventoRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(evento);

            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.Eventos).Returns(mockEventos.Object);
            mockUoW.Setup(u => u.EventoRepository).Returns(mockEventoRepo.Object);

            return mockUoW;
        }

        [Fact]
        public async Task ObtenerEventoConDetallesAsync_CuandoExiste_DevuelveEvento()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1) { Id = 1 };
            var mockUoW = CreateUnitOfWorkMock(evento);

            var service = new EventoService(mockUoW.Object);

            var resultado = await service.ObtenerEventoConDetallesAsync(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
        }

        [Fact]
        public async Task ObtenerEventoConDetallesAsync_CuandoNoExiste_DevuelveNull()
        {
            var mockUoW = CreateUnitOfWorkMock(evento: null);
            var service = new EventoService(mockUoW.Object);

            var resultado = await service.ObtenerEventoConDetallesAsync(999);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task ActualizarAsync_CuandoEventoExiste_ActualizaDatos()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1) { Id = 1 };
            var mockUoW = CreateUnitOfWorkMock(evento);
            var service = new EventoService(mockUoW.Object);

            var request = new EditarEventoRequest
            {
                Id = 1,
                Name = "Hackathon Actualizado",
                FechaInicio = DateTime.UtcNow,
                FechaFin = DateTime.UtcNow.AddDays(2),
                Description = "Nueva descripción"
            };

            await service.ActualizarAsync(request);

            Assert.Equal("Hackathon Actualizado", evento.Name);
            mockUoW.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ActualizarAsync_CuandoEventoNoExiste_LanzaExcepcion()
        {
            var mockUoW = CreateUnitOfWorkMock(evento: null);
            var service = new EventoService(mockUoW.Object);

            var request = new EditarEventoRequest
            {
                Id = 999,
                Name = "Hackathon",
                FechaInicio = DateTime.UtcNow,
                FechaFin = DateTime.UtcNow.AddDays(1),
                Description = "Descripción"
            };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.ActualizarAsync(request));
        }
    }
}
