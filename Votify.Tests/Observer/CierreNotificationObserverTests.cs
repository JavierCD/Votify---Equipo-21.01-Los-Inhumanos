using Moq;
using Votify.Core.Enums;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Implementations.Observers;
using Xunit;

namespace Votify.Tests.Observer
{
    public class CierreNotificationObserverTests
    {
        [Fact]
        public async Task HandleAsync_CuandoEventoEsCierre_CreaNotificacionesParaJueces()
        {
            var juez = new Juez { Id = 1, Name = "Juez1", QuiereRecibirNotificaciones = true };
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez> { juez };
            var categoria = new Categoria { Id = 1, Name = "IA", Evento = evento };

            var votacion = new Popular { Id = 1 };
            votacion.Categoria = categoria;

            var args = new VotacionStateChangedArgs
            {
                EventType = VotacionStateEventType.Cierre,
                Votacion = votacion,
                Evento = evento
            };

            var mockNotificaciones = new Mock<IGenericRepository<Notificacion>>();
            var mockVotaciones = new Mock<IGenericRepository<Votacion>>();
            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.Notificaciones).Returns(mockNotificaciones.Object);
            mockUoW.Setup(u => u.Votaciones).Returns(mockVotaciones.Object);

            var observer = new CierreNotificationObserver(mockUoW.Object);

            await observer.HandleAsync(args);

            mockNotificaciones.Verify(
                r => r.AddAsync(It.Is<Notificacion>(n => n.MiembroId == juez.Id && n.Titulo == "Votación Cerrada")),
                Times.Once
            );
            mockVotaciones.Verify(r => r.UpdateAsync(votacion), Times.Once);
            Assert.True(votacion.NotificacionCierreEnviada);
        }

        [Fact]
        public async Task HandleAsync_CuandoEventoEsApertura_NoHaceNada()
        {
            var args = new VotacionStateChangedArgs { EventType = VotacionStateEventType.Apertura };

            var mockUoW = new Mock<IUnitOfWork>();
            var observer = new CierreNotificationObserver(mockUoW.Object);

            await observer.HandleAsync(args);

            mockUoW.Verify(u => u.Notificaciones.AddAsync(It.IsAny<Notificacion>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_CuandoJuezNoQuiereNotificaciones_NoCreaNotificacion()
        {
            var juez = new Juez { Id = 1, Name = "Juez1", QuiereRecibirNotificaciones = false };
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez> { juez };
            var categoria = new Categoria { Id = 1, Name = "IA", Evento = evento };
            var votacion = new Popular { Id = 1 };
            votacion.Categoria = categoria;

            var args = new VotacionStateChangedArgs
            {
                EventType = VotacionStateEventType.Cierre,
                Votacion = votacion,
                Evento = evento
            };

            var mockNotificaciones = new Mock<IGenericRepository<Notificacion>>();
            var mockVotaciones = new Mock<IGenericRepository<Votacion>>();
            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.Notificaciones).Returns(mockNotificaciones.Object);
            mockUoW.Setup(u => u.Votaciones).Returns(mockVotaciones.Object);

            var observer = new CierreNotificationObserver(mockUoW.Object);

            await observer.HandleAsync(args);

            mockNotificaciones.Verify(u => u.AddAsync(It.IsAny<Notificacion>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_CuandoEventoSinJurado_NoCreaNotificaciones()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez>();
            var categoria = new Categoria { Id = 1, Name = "IA", Evento = evento };
            var votacion = new Popular { Id = 1 };
            votacion.Categoria = categoria;

            var args = new VotacionStateChangedArgs
            {
                EventType = VotacionStateEventType.Cierre,
                Votacion = votacion,
                Evento = evento
            };

            var mockUoW = new Mock<IUnitOfWork>();
            var observer = new CierreNotificationObserver(mockUoW.Object);

            await observer.HandleAsync(args);

            mockUoW.Verify(u => u.Notificaciones.AddAsync(It.IsAny<Notificacion>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_CuandoHayVariosJueces_CreaUnaNotificacionPorCadaUno()
        {
            var juez1 = new Juez { Id = 1, QuiereRecibirNotificaciones = true };
            var juez2 = new Juez { Id = 2, QuiereRecibirNotificaciones = true };
            var juez3 = new Juez { Id = 3, QuiereRecibirNotificaciones = true };
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez> { juez1, juez2, juez3 };
            var categoria = new Categoria { Id = 1, Name = "IA", Evento = evento };
            var votacion = new Popular { Id = 1 };
            votacion.Categoria = categoria;

            var args = new VotacionStateChangedArgs
            {
                EventType = VotacionStateEventType.Cierre,
                Votacion = votacion,
                Evento = evento
            };

            var mockNotificaciones = new Mock<IGenericRepository<Notificacion>>();
            var mockVotaciones = new Mock<IGenericRepository<Votacion>>();
            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.Notificaciones).Returns(mockNotificaciones.Object);
            mockUoW.Setup(u => u.Votaciones).Returns(mockVotaciones.Object);

            var observer = new CierreNotificationObserver(mockUoW.Object);

            await observer.HandleAsync(args);

            mockNotificaciones.Verify(r => r.AddAsync(It.IsAny<Notificacion>()), Times.Exactly(3));
        }

        [Fact]
        public async Task HandleAsync_CuandoCierre_MarcaVotacionComoCerrada()
        {
            var juez = new Juez { Id = 1, QuiereRecibirNotificaciones = true };
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez> { juez };
            var categoria = new Categoria { Id = 1, Name = "IA", Evento = evento };
            var votacion = new Popular { Id = 1 };
            votacion.Categoria = categoria;

            var args = new VotacionStateChangedArgs
            {
                EventType = VotacionStateEventType.Cierre,
                Votacion = votacion,
                Evento = evento
            };

            var mockNotificaciones = new Mock<IGenericRepository<Notificacion>>();
            var mockVotaciones = new Mock<IGenericRepository<Votacion>>();
            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.Notificaciones).Returns(mockNotificaciones.Object);
            mockUoW.Setup(u => u.Votaciones).Returns(mockVotaciones.Object);

            var observer = new CierreNotificationObserver(mockUoW.Object);

            await observer.HandleAsync(args);

            Assert.True(votacion.EstaCerrada);
        }
    }
}
