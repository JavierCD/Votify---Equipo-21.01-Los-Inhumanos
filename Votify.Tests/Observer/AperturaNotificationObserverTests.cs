using Moq;
using Votify.Core.Enums;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Implementations.Observers;
using Xunit;

namespace Votify.Tests.Observer
{
    public class AperturaNotificationObserverTests
    {
        [Fact]
        public async Task HandleAsync_CuandoEventoEsApertura_CreaNotificacionesParaJueces()
        {
            // ARRANGE
            var juez = new Juez { Id = 1, Name = "Juez1", QuiereRecibirNotificaciones = true };
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez> { juez };
            var categoria = new Categoria { Id = 1, Name = "IA", Evento = evento };

            var votacion = new Popular { Id = 1, EnviarNotificacionApertura = true };
            votacion.Categoria = categoria;
            

            var args = new VotacionStateChangedArgs
            {
                EventType = VotacionStateEventType.Apertura,
                Votacion = votacion,
                Evento = evento
            };

            var mockNotificaciones = new Mock<IGenericRepository<Notificacion>>();
            var mockVotaciones = new Mock<IGenericRepository<Votacion>>();
            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.Notificaciones).Returns(mockNotificaciones.Object);
            mockUoW.Setup(u => u.Votaciones).Returns(mockVotaciones.Object);

            var observer = new AperturaNotificationObserver(mockUoW.Object);

            // ACT
            await observer.HandleAsync(args);

            // ASSERT
            mockNotificaciones.Verify(
                r => r.AddAsync(It.Is<Notificacion>(n => n.MiembroId == juez.Id && n.Titulo == "Votación abierta")),
                Times.Once
            );
            mockVotaciones.Verify(r => r.UpdateAsync(votacion), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_CuandoEventoEsCierre_NoHaceNada()
        {
            // ARRANGE
            var args = new VotacionStateChangedArgs { EventType = VotacionStateEventType.Cierre };

            var mockUoW = new Mock<IUnitOfWork>();
            var observer = new AperturaNotificationObserver(mockUoW.Object);

            // ACT
            await observer.HandleAsync(args);

            // ASSERT
            mockUoW.Verify(u => u.Notificaciones.AddAsync(It.IsAny<Notificacion>()), Times.Never);
        }

        [Fact]
        
        public async Task HandleAsync_CuandoEnviarNotificacionAperturaEsFalse_NoCreaNotificaciones()
        {
            // ARRANGE
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez> { new Juez { Id = 1, QuiereRecibirNotificaciones = true } };

            var categoria = new Categoria { Id = 1, Name = "IA", Evento = evento };

            var votacion = new Popular { Id = 1, EnviarNotificacionApertura = false };
            votacion.Categoria = categoria;

            var args = new VotacionStateChangedArgs
            {
                EventType = VotacionStateEventType.Apertura,
                Votacion = votacion,
                Evento = evento
            };

            var mockUoW = new Mock<IUnitOfWork>();
            var observer = new AperturaNotificationObserver(mockUoW.Object);

            // ACT
            await observer.HandleAsync(args);

            // ASSERT
            mockUoW.Verify(u => u.Notificaciones.AddAsync(It.IsAny<Notificacion>()), Times.Never);
        }
    }
}