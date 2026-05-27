using System.Linq.Expressions;
using Moq;
using Votify.Core.Enums;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Implementations;
using Xunit;

namespace Votify.Tests.Observer
{
    public class VotacionStateCronDetectorTests
    {
        private static Mock<IUnitOfWork> SetupMocks(List<Votacion> votaciones, Evento? evento = null)
        {
            var mockVotaciones = new Mock<IGenericRepository<Votacion>>();
            mockVotaciones
                .Setup(r => r.GetAllWithIncludesAsync(
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>()))
                .ReturnsAsync(votaciones);

            var mockEventos = new Mock<IGenericRepository<Evento>>();
            mockEventos
                .Setup(r => r.GetWithIncludesAsync(
                    It.IsAny<Expression<Func<Evento, bool>>>(),
                    It.IsAny<Expression<Func<Evento, object>>>()))
                .ReturnsAsync(evento);

            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.Votaciones).Returns(mockVotaciones.Object);
            mockUoW.Setup(u => u.Eventos).Returns(mockEventos.Object);

            return mockUoW;
        }

        [Fact]
        public async Task DetectAndNotifyAsync_CuandoHayAperturaPendiente_NotificaApertura()
        {
            var ahora = DateTime.UtcNow;
            var evento = new HackathonEvent("Hackathon", ahora, ahora.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez>();
            var categoria = new Categoria { Id = 1, Name = "IA", EventoId = 1, Evento = evento };
            var votacion = new Popular
            {
                Id = 1,
                FechaApertura = ahora.AddMinutes(-1),
                FechaCierre = ahora.AddDays(1),
                EnviarNotificacionApertura = true,
                NotificacionAperturaEnviada = false,
                Estado = EstadoVotacion.Programada
            };
            votacion.Categoria = categoria;

            var mockUoW = SetupMocks(new List<Votacion> { votacion }, evento);
            var mockSubject = new Mock<IVotacionStateSubject>();
            var detector = new VotacionStateCronDetector(mockUoW.Object, mockSubject.Object);

            await detector.DetectAndNotifyAsync();

            mockSubject.Verify(
                s => s.NotifyAsync(It.Is<VotacionStateChangedArgs>(
                    a => a.EventType == VotacionStateEventType.Apertura && a.Votacion.Id == 1)),
                Times.Once
            );
            Assert.Equal(EstadoVotacion.Abierta, votacion.Estado);
        }

        [Fact]
        public async Task DetectAndNotifyAsync_CuandoNoHayAperturasPendientes_NoNotificaApertura()
        {
            var ahora = DateTime.UtcNow;
            var evento = new HackathonEvent("Hackathon", ahora, ahora.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez>();
            var categoria = new Categoria { Id = 1, Name = "IA", EventoId = 1, Evento = evento };
            var votacion = new Popular
            {
                Id = 1,
                FechaApertura = ahora.AddMinutes(10),
                EnviarNotificacionApertura = true,
                NotificacionAperturaEnviada = false,
                Estado = EstadoVotacion.Programada
            };
            votacion.Categoria = categoria;

            var mockUoW = SetupMocks(new List<Votacion> { votacion }, evento);
            var mockSubject = new Mock<IVotacionStateSubject>();
            var detector = new VotacionStateCronDetector(mockUoW.Object, mockSubject.Object);

            await detector.DetectAndNotifyAsync();

            mockSubject.Verify(
                s => s.NotifyAsync(It.Is<VotacionStateChangedArgs>(
                    a => a.EventType == VotacionStateEventType.Apertura)),
                Times.Never
            );
        }

        [Fact]
        public async Task DetectAndNotifyAsync_CuandoCierreEsProximo_NotificaRecordatorio()
        {
            var ahora = DateTime.UtcNow;
            var evento = new HackathonEvent("Hackathon", ahora, ahora.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez>();
            var categoria = new Categoria { Id = 1, Name = "IA", EventoId = 1, Evento = evento };
            var votacion = new Popular
            {
                Id = 1,
                FechaCierre = ahora.AddMinutes(3),
                Estado = EstadoVotacion.Abierta,
                NotificacionRecordatorioEnviada = false
            };
            votacion.Categoria = categoria;

            var mockUoW = SetupMocks(new List<Votacion> { votacion }, evento);
            var mockSubject = new Mock<IVotacionStateSubject>();
            var detector = new VotacionStateCronDetector(mockUoW.Object, mockSubject.Object);

            await detector.DetectAndNotifyAsync();

            mockSubject.Verify(
                s => s.NotifyAsync(It.Is<VotacionStateChangedArgs>(
                    a => a.EventType == VotacionStateEventType.ProximoCierre && a.Votacion.Id == 1)),
                Times.Once
            );
        }

        [Fact]
        public async Task DetectAndNotifyAsync_CuandoCierreYaPaso_NoNotificaRecordatorio()
        {
            var ahora = DateTime.UtcNow;
            var evento = new HackathonEvent("Hackathon", ahora, ahora.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez>();
            var categoria = new Categoria { Id = 1, Name = "IA", EventoId = 1, Evento = evento };
            var votacion = new Popular
            {
                Id = 1,
                FechaCierre = ahora.AddMinutes(-10),
                Estado = EstadoVotacion.Abierta,
                NotificacionRecordatorioEnviada = false
            };
            votacion.Categoria = categoria;

            var mockUoW = SetupMocks(new List<Votacion> { votacion }, evento);
            var mockSubject = new Mock<IVotacionStateSubject>();
            var detector = new VotacionStateCronDetector(mockUoW.Object, mockSubject.Object);

            await detector.DetectAndNotifyAsync();

            mockSubject.Verify(
                s => s.NotifyAsync(It.Is<VotacionStateChangedArgs>(
                    a => a.EventType == VotacionStateEventType.ProximoCierre)),
                Times.Never
            );
        }

        [Fact]
        public async Task DetectAndNotifyAsync_CuandoVotacionEstaAbiertaYFechaCierrePaso_NotificaCierre()
        {
            var ahora = DateTime.UtcNow;
            var evento = new HackathonEvent("Hackathon", ahora, ahora.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez>();
            var categoria = new Categoria { Id = 1, Name = "IA", EventoId = 1, Evento = evento };
            var votacion = new Popular
            {
                Id = 1,
                FechaCierre = ahora.AddMinutes(-10),
                Estado = EstadoVotacion.Abierta,
                NotificacionCierreEnviada = false,
                EnviarNotificacionApertura = false
            };
            votacion.Categoria = categoria;

            var mockUoW = SetupMocks(new List<Votacion> { votacion }, evento);
            var mockSubject = new Mock<IVotacionStateSubject>();
            var detector = new VotacionStateCronDetector(mockUoW.Object, mockSubject.Object);

            await detector.DetectAndNotifyAsync();

            mockSubject.Verify(
                s => s.NotifyAsync(It.Is<VotacionStateChangedArgs>(
                    a => a.EventType == VotacionStateEventType.Cierre && a.Votacion.Id == 1)),
                Times.Once
            );
        }

        [Fact]
        public async Task DetectAndNotifyAsync_CuandoVotacionNoEstaAbierta_NoNotificaCierre()
        {
            var ahora = DateTime.UtcNow;
            var evento = new HackathonEvent("Hackathon", ahora, ahora.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez>();
            var categoria = new Categoria { Id = 1, Name = "IA", EventoId = 1, Evento = evento };
            var votacion = new Popular
            {
                Id = 1,
                FechaCierre = ahora.AddMinutes(-1),
                Estado = EstadoVotacion.Cerrada,
                NotificacionCierreEnviada = false,
                EnviarNotificacionApertura = false
            };
            votacion.Categoria = categoria;

            var mockUoW = SetupMocks(new List<Votacion> { votacion }, evento);
            var mockSubject = new Mock<IVotacionStateSubject>();
            var detector = new VotacionStateCronDetector(mockUoW.Object, mockSubject.Object);

            await detector.DetectAndNotifyAsync();

            mockSubject.Verify(
                s => s.NotifyAsync(It.Is<VotacionStateChangedArgs>(
                    a => a.EventType == VotacionStateEventType.Cierre)),
                Times.Never
            );
        }

        [Fact]
        public async Task DetectAndNotifyAsync_CuandoNotificacionAperturaYaFueEnviada_NoNotificaDeNuevo()
        {
            var ahora = DateTime.UtcNow;
            var evento = new HackathonEvent("Hackathon", ahora, ahora.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez>();
            var categoria = new Categoria { Id = 1, Name = "IA", EventoId = 1, Evento = evento };
            var votacion = new Popular
            {
                Id = 1,
                FechaApertura = ahora.AddMinutes(-1),
                EnviarNotificacionApertura = true,
                NotificacionAperturaEnviada = true,
                Estado = EstadoVotacion.Programada
            };
            votacion.Categoria = categoria;

            var mockUoW = SetupMocks(new List<Votacion> { votacion }, evento);
            var mockSubject = new Mock<IVotacionStateSubject>();
            var detector = new VotacionStateCronDetector(mockUoW.Object, mockSubject.Object);

            await detector.DetectAndNotifyAsync();

            mockSubject.Verify(
                s => s.NotifyAsync(It.Is<VotacionStateChangedArgs>(
                    a => a.EventType == VotacionStateEventType.Apertura)),
                Times.Never
            );
        }

        [Fact]
        public async Task DetectAndNotifyAsync_CuandoRecordatorioYaFueEnviado_NoNotificaDeNuevo()
        {
            var ahora = DateTime.UtcNow;
            var evento = new HackathonEvent("Hackathon", ahora, ahora.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez>();
            var categoria = new Categoria { Id = 1, Name = "IA", EventoId = 1, Evento = evento };
            var votacion = new Popular
            {
                Id = 1,
                FechaCierre = ahora.AddMinutes(3),
                Estado = EstadoVotacion.Abierta,
                NotificacionRecordatorioEnviada = true
            };
            votacion.Categoria = categoria;

            var mockUoW = SetupMocks(new List<Votacion> { votacion }, evento);
            var mockSubject = new Mock<IVotacionStateSubject>();
            var detector = new VotacionStateCronDetector(mockUoW.Object, mockSubject.Object);

            await detector.DetectAndNotifyAsync();

            mockSubject.Verify(
                s => s.NotifyAsync(It.Is<VotacionStateChangedArgs>(
                    a => a.EventType == VotacionStateEventType.ProximoCierre)),
                Times.Never
            );
        }

        [Fact]
        public async Task DetectAndNotifyAsync_CuandoCierreYaFueNotificado_NoNotificaDeNuevo()
        {
            var ahora = DateTime.UtcNow;
            var evento = new HackathonEvent("Hackathon", ahora, ahora.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez>();
            var categoria = new Categoria { Id = 1, Name = "IA", EventoId = 1, Evento = evento };
            var votacion = new Popular
            {
                Id = 1,
                FechaCierre = ahora.AddMinutes(-1),
                Estado = EstadoVotacion.Abierta,
                NotificacionCierreEnviada = true
            };
            votacion.Categoria = categoria;

            var mockUoW = SetupMocks(new List<Votacion> { votacion }, evento);
            var mockSubject = new Mock<IVotacionStateSubject>();
            var detector = new VotacionStateCronDetector(mockUoW.Object, mockSubject.Object);

            await detector.DetectAndNotifyAsync();

            mockSubject.Verify(
                s => s.NotifyAsync(It.Is<VotacionStateChangedArgs>(
                    a => a.EventType == VotacionStateEventType.Cierre)),
                Times.Never
            );
        }

        [Fact]
        public async Task DetectAndNotifyAsync_CuandoNoHayVotaciones_NoNotificaNada()
        {
            var mockUoW = SetupMocks(new List<Votacion>());
            var mockSubject = new Mock<IVotacionStateSubject>();
            var detector = new VotacionStateCronDetector(mockUoW.Object, mockSubject.Object);

            await detector.DetectAndNotifyAsync();

            mockSubject.Verify(s => s.NotifyAsync(It.IsAny<VotacionStateChangedArgs>()), Times.Never);
            mockUoW.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
