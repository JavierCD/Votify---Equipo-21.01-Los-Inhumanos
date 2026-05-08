using Moq;
using Votify.Core.Enums;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Implementations.Observers;
using Xunit;

namespace Votify.Tests.Observer
{
    public class RecordatorioObserverTests
    {
        [Fact]
        public async Task HandleAsync_CuandoEventoEsProximoCierre_CreaRecordatoriosParaJuecesSinVotar()
        {
            var juez1 = new Juez { Id = 1, Name = "Juez1", QuiereRecibirNotificaciones = true };
            var juez2 = new Juez { Id = 2, Name = "Juez2", QuiereRecibirNotificaciones = true };
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez> { juez1, juez2 };
            var categoria = new Categoria { Id = 1, Name = "IA", Evento = evento };
            var votacion = new Popular { Id = 1 };
            votacion.Categoria = categoria;
            votacion.Votos = new List<Voto>();

            var args = new VotacionStateChangedArgs
            {
                EventType = VotacionStateEventType.ProximoCierre,
                Votacion = votacion,
                Evento = evento
            };

            var mockNotificaciones = new Mock<IGenericRepository<Notificacion>>();
            var mockVotaciones = new Mock<IGenericRepository<Votacion>>();
            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.Notificaciones).Returns(mockNotificaciones.Object);
            mockUoW.Setup(u => u.Votaciones).Returns(mockVotaciones.Object);

            var observer = new RecordatorioObserver(mockUoW.Object);

            await observer.HandleAsync(args);

            mockNotificaciones.Verify(r => r.AddAsync(It.IsAny<Notificacion>()), Times.Exactly(2));
            mockVotaciones.Verify(r => r.UpdateAsync(votacion), Times.Once);
            Assert.True(votacion.NotificacionRecordatorioEnviada);
        }

        [Fact]
        public async Task HandleAsync_CuandoEventoEsApertura_NoHaceNada()
        {
            var args = new VotacionStateChangedArgs { EventType = VotacionStateEventType.Apertura };

            var mockUoW = new Mock<IUnitOfWork>();
            var observer = new RecordatorioObserver(mockUoW.Object);

            await observer.HandleAsync(args);

            mockUoW.Verify(u => u.Notificaciones.AddAsync(It.IsAny<Notificacion>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_CuandoEventoEsCierre_NoHaceNada()
        {
            var args = new VotacionStateChangedArgs { EventType = VotacionStateEventType.Cierre };

            var mockUoW = new Mock<IUnitOfWork>();
            var observer = new RecordatorioObserver(mockUoW.Object);

            await observer.HandleAsync(args);

            mockUoW.Verify(u => u.Notificaciones.AddAsync(It.IsAny<Notificacion>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_CuandoJuezNoQuiereNotificaciones_NoCreaRecordatorio()
        {
            var juez = new Juez { Id = 1, Name = "Juez1", QuiereRecibirNotificaciones = false };
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez> { juez };
            var categoria = new Categoria { Id = 1, Name = "IA", Evento = evento };
            var votacion = new Popular { Id = 1 };
            votacion.Categoria = categoria;
            votacion.Votos = new List<Voto>();

            var args = new VotacionStateChangedArgs
            {
                EventType = VotacionStateEventType.ProximoCierre,
                Votacion = votacion,
                Evento = evento
            };

            var mockNotificaciones = new Mock<IGenericRepository<Notificacion>>();
            var mockVotaciones = new Mock<IGenericRepository<Votacion>>();
            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.Notificaciones).Returns(mockNotificaciones.Object);
            mockUoW.Setup(u => u.Votaciones).Returns(mockVotaciones.Object);

            var observer = new RecordatorioObserver(mockUoW.Object);

            await observer.HandleAsync(args);

            mockNotificaciones.Verify(u => u.AddAsync(It.IsAny<Notificacion>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_CuandoJuezYaVoto_NoCreaRecordatorio()
        {
            var juez1 = new Juez { Id = 1, Name = "Juez1", QuiereRecibirNotificaciones = true };
            var juez2 = new Juez { Id = 2, Name = "Juez2", QuiereRecibirNotificaciones = true };
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez> { juez1, juez2 };
            var categoria = new Categoria { Id = 1, Name = "IA", Evento = evento };
            var votacion = new Popular { Id = 1 };
            votacion.Categoria = categoria;
            votacion.Votos = new List<Voto>
            {
                new VotoExperto(1, 1, 5.0) { JuezId = juez1.Id }
            };

            var args = new VotacionStateChangedArgs
            {
                EventType = VotacionStateEventType.ProximoCierre,
                Votacion = votacion,
                Evento = evento
            };

            var mockNotificaciones = new Mock<IGenericRepository<Notificacion>>();
            var mockVotaciones = new Mock<IGenericRepository<Votacion>>();
            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.Notificaciones).Returns(mockNotificaciones.Object);
            mockUoW.Setup(u => u.Votaciones).Returns(mockVotaciones.Object);

            var observer = new RecordatorioObserver(mockUoW.Object);

            await observer.HandleAsync(args);

            mockNotificaciones.Verify(r => r.AddAsync(It.IsAny<Notificacion>()), Times.Exactly(1));
        }

        [Fact]
        public async Task HandleAsync_CuandoEventoSinJurado_NoCreaRecordatorios()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez>();
            var categoria = new Categoria { Id = 1, Name = "IA", Evento = evento };
            var votacion = new Popular { Id = 1 };
            votacion.Categoria = categoria;

            var args = new VotacionStateChangedArgs
            {
                EventType = VotacionStateEventType.ProximoCierre,
                Votacion = votacion,
                Evento = evento
            };

            var mockUoW = new Mock<IUnitOfWork>();
            var observer = new RecordatorioObserver(mockUoW.Object);

            await observer.HandleAsync(args);

            mockUoW.Verify(u => u.Notificaciones.AddAsync(It.IsAny<Notificacion>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_CuandoTodosLosJuecesVotaron_NoCreaRecordatorios()
        {
            var juez1 = new Juez { Id = 1, Name = "Juez1", QuiereRecibirNotificaciones = true };
            var juez2 = new Juez { Id = 2, Name = "Juez2", QuiereRecibirNotificaciones = true };
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez> { juez1, juez2 };
            var categoria = new Categoria { Id = 1, Name = "IA", Evento = evento };
            var votacion = new Popular { Id = 1 };
            votacion.Categoria = categoria;
            votacion.Votos = new List<Voto>
            {
                new VotoExperto(1, 1, 5.0) { JuezId = juez1.Id },
                new VotoExperto(1, 1, 5.0) { JuezId = juez2.Id }
            };

            var args = new VotacionStateChangedArgs
            {
                EventType = VotacionStateEventType.ProximoCierre,
                Votacion = votacion,
                Evento = evento
            };

            var mockNotificaciones = new Mock<IGenericRepository<Notificacion>>();
            var mockVotaciones = new Mock<IGenericRepository<Votacion>>();
            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.Notificaciones).Returns(mockNotificaciones.Object);
            mockUoW.Setup(u => u.Votaciones).Returns(mockVotaciones.Object);

            var observer = new RecordatorioObserver(mockUoW.Object);

            await observer.HandleAsync(args);

            mockNotificaciones.Verify(r => r.AddAsync(It.IsAny<Notificacion>()), Times.Never);
        }
    }
}
