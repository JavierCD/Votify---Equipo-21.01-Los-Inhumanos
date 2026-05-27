using Moq;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Core.Enums;
using Votify.Services.Implementations;
using Xunit;

namespace Votify.Tests.Services
{
    public class VotacionServiceTests
    {
        private Mock<IUnitOfWork> CreateUnitOfWorkMock(Votacion? votacion = null)
        {
            var mockVotaciones = new Mock<IGenericRepository<Votacion>>();
            mockVotaciones.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(votacion);
            mockVotaciones.Setup(r => r.GetWithIncludesAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Votacion, bool>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<Votacion, object>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<Votacion, object>>>(), It.IsAny<System.Linq.Expressions.Expression<Func<Votacion, object>>>()))
                .ReturnsAsync(votacion);

            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.Votaciones).Returns(mockVotaciones.Object);

            return mockUoW;
        }

        private Mock<IVotacionStateSubject> CreateSubjectMock()
        {
            return new Mock<IVotacionStateSubject>();
        }

        [Fact]
        public async Task ActualizarFechasVotacionAsync_CuandoVotacionExiste_ActualizaFechas()
        {
            var votacion = new Puntuacion { Id = 1 };
            var mockUoW = CreateUnitOfWorkMock(votacion);
            var subjectMock = CreateSubjectMock();

            var service = new VotacionService(mockUoW.Object, subjectMock.Object);

            var nuevaApertura = DateTime.UtcNow.AddDays(1);
            var nuevoCierre = DateTime.UtcNow.AddDays(2);

            await service.ActualizarFechasVotacionAsync(1, nuevaApertura, nuevoCierre);

            Assert.Equal(nuevaApertura, votacion.FechaApertura);
            Assert.Equal(nuevoCierre, votacion.FechaCierre);
            mockUoW.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ActualizarFechasVotacionAsync_CuandoVotacionNoExiste_LanzaExcepcion()
        {
            var mockUoW = CreateUnitOfWorkMock(votacion: null);
            var subjectMock = CreateSubjectMock();

            var service = new VotacionService(mockUoW.Object, subjectMock.Object);

            await Assert.ThrowsAsync<Exception>(() => service.ActualizarFechasVotacionAsync(999, DateTime.UtcNow, DateTime.UtcNow.AddDays(1)));
        }

        [Fact]
        public async Task CambiarEstadoVotacionManualAsync_CuandoVotacionNoExiste_DevuelveFalse()
        {
            var mockUoW = CreateUnitOfWorkMock(votacion: null);
            var subjectMock = CreateSubjectMock();

            var service = new VotacionService(mockUoW.Object, subjectMock.Object);

            var resultado = await service.CambiarEstadoVotacionManualAsync(999, EstadoVotacion.Abierta);

            Assert.False(resultado);
        }

        [Fact]
        public async Task CambiarEstadoVotacionManualAsync_CuandoEstadoAbierta_CambiaEstadoYNotifica()
        {
            var evento = new HackathonEvent("Test", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez>();
            var categoria = new Categoria { Id = 1, Name = "Test", Evento = evento };
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Programada };
            votacion.Categoria = categoria;

            var mockUoW = CreateUnitOfWorkMock(votacion);
            var subjectMock = CreateSubjectMock();

            var service = new VotacionService(mockUoW.Object, subjectMock.Object);

            var resultado = await service.CambiarEstadoVotacionManualAsync(1, EstadoVotacion.Abierta);

            Assert.True(resultado);
            Assert.Equal(EstadoVotacion.Abierta, votacion.Estado);
            subjectMock.Verify(s => s.NotifyAsync(It.Is<VotacionStateChangedArgs>(a => a.EventType == Votify.Core.Enums.VotacionStateEventType.Apertura)), Times.Once);
        }

        [Fact]
        public async Task CambiarEstadoVotacionManualAsync_CuandoEstadoCerrada_CambiaEstadoYNotifica()
        {
            var evento = new HackathonEvent("Test", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez>();
            var categoria = new Categoria { Id = 1, Name = "Test", Evento = evento };
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Abierta };
            votacion.Categoria = categoria;

            var mockUoW = CreateUnitOfWorkMock(votacion);
            var subjectMock = CreateSubjectMock();

            var service = new VotacionService(mockUoW.Object, subjectMock.Object);

            var resultado = await service.CambiarEstadoVotacionManualAsync(1, EstadoVotacion.Cerrada);

            Assert.True(resultado);
            Assert.Equal(EstadoVotacion.Cerrada, votacion.Estado);
            subjectMock.Verify(s => s.NotifyAsync(It.Is<VotacionStateChangedArgs>(a => a.EventType == Votify.Core.Enums.VotacionStateEventType.Cierre)), Times.Once);
        }

        [Fact]
        public async Task CambiarEstadoVotacionManualAsync_CuandoEstadoPausada_CambiaEstado()
        {
            var evento = new HackathonEvent("Test", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez>();
            var categoria = new Categoria { Id = 1, Name = "Test", Evento = evento };
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Abierta };
            votacion.Categoria = categoria;

            var mockUoW = CreateUnitOfWorkMock(votacion);
            var subjectMock = CreateSubjectMock();

            var service = new VotacionService(mockUoW.Object, subjectMock.Object);

            var resultado = await service.CambiarEstadoVotacionManualAsync(1, EstadoVotacion.Pausada);

            Assert.True(resultado);
            Assert.Equal(EstadoVotacion.Pausada, votacion.Estado);
        }

        [Fact]
        public async Task CambiarEstadoVotacionManualAsync_CuandoEstadoProgramada_CambiaEstado()
        {
            var evento = new HackathonEvent("Test", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez>();
            var categoria = new Categoria { Id = 1, Name = "Test", Evento = evento };
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Pausada };
            votacion.Categoria = categoria;

            var mockUoW = CreateUnitOfWorkMock(votacion);
            var subjectMock = CreateSubjectMock();

            var service = new VotacionService(mockUoW.Object, subjectMock.Object);

            var resultado = await service.CambiarEstadoVotacionManualAsync(1, EstadoVotacion.Programada);

            Assert.True(resultado);
            Assert.Equal(EstadoVotacion.Programada, votacion.Estado);
        }
    }
}
