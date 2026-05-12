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
        [Fact]
        public async Task DetectAndNotifyAsync_CuandoHayAperturaPendiente_NotificaApertura()
        {
            var ahora = DateTime.UtcNow;
            var evento = new HackathonEvent("Hackathon", ahora, ahora.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez>();
            var categoria = new Categoria { Id = 1, Name = "IA", Evento = evento };
            var votacion = new Popular
            {
                Id = 1,
                FechaApertura = ahora.AddMinutes(-1),
                EnviarNotificacionApertura = true,
                NotificacionAperturaEnviada = false,
                Estado = "Pendiente"
            };
            votacion.Categoria = categoria;

            var mockVotaciones = new Mock<IGenericRepository<Votacion>>();
            mockVotaciones
                .Setup(r => r.GetAllWithIncludesAsync(
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>()))
                .ReturnsAsync(new List<Votacion> { votacion });

            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.Votaciones).Returns(mockVotaciones.Object);

            var mockSubject = new Mock<IVotacionStateSubject>();
            var detector = new VotacionStateCronDetector(mockUoW.Object, mockSubject.Object);

            await detector.DetectAndNotifyAsync();

            mockSubject.Verify(
                s => s.NotifyAsync(It.Is<VotacionStateChangedArgs>(
                    a => a.EventType == VotacionStateEventType.Apertura && a.Votacion.Id == 1)),
                Times.Once
            );
            mockVotaciones.Verify(r => r.UpdateAsync(votacion), Times.Once);
            Assert.Equal("Abierta", votacion.Estado);
        }

        [Fact]
        public async Task DetectAndNotifyAsync_CuandoNoHayAperturasPendientes_NoNotificaApertura()
        {
            var ahora = DateTime.UtcNow;
            var evento = new HackathonEvent("Hackathon", ahora, ahora.AddDays(1), 1);
            evento.Id = 1;
            evento.Jurado = new List<Juez>();
            var categoria = new Categoria { Id = 1, Name = "IA", Evento = evento };
            var votacion = new Popular
            {
                Id = 1,
                FechaApertura = ahora.AddMinutes(10),
                EnviarNotificacionApertura = true,
                NotificacionAperturaEnviada = false,
                Estado = "Pendiente"
            };
            votacion.Categoria = categoria;

            var mockVotaciones = new Mock<IGenericRepository<Votacion>>();
            mockVotaciones
                .Setup(r => r.GetAllWithIncludesAsync(
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>()))
                .ReturnsAsync(new List<Votacion> { votacion });

            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.Votaciones).Returns(mockVotaciones.Object);

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
            var categoria = new Categoria { Id = 1, Name = "IA", Evento = evento };
            var votacion = new Popular
            {
                Id = 1,
                FechaCierre = ahora.AddMinutes(3),
                Estado = "Abierta",
                NotificacionRecordatorioEnviada = false
            };
            votacion.Categoria = categoria;

            var mockVotaciones = new Mock<IGenericRepository<Votacion>>();
            mockVotaciones
                .Setup(r => r.GetAllWithIncludesAsync(
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>()))
                .ReturnsAsync(new List<Votacion> { votacion });

            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.Votaciones).Returns(mockVotaciones.Object);

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
            var categoria = new Categoria { Id = 1, Name = "IA", Evento = evento };
            var votacion = new Popular
            {
                Id = 1,
                FechaCierre = ahora.AddMinutes(-10),
                Estado = "Abierta",
                NotificacionRecordatorioEnviada = false
            };
            votacion.Categoria = categoria;

            var mockVotaciones = new Mock<IGenericRepository<Votacion>>();
            mockVotaciones
                .Setup(r => r.GetAllWithIncludesAsync(
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>()))
                .ReturnsAsync(new List<Votacion> { votacion });

            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.Votaciones).Returns(mockVotaciones.Object);

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
            var categoria = new Categoria { Id = 1, Name = "IA", Evento = evento };
            var votacion = new Popular
            {
                Id = 1,
                FechaCierre = ahora.AddMinutes(-1),
                Estado = "Abierta",
                NotificacionCierreEnviada = false
            };
            votacion.Categoria = categoria;

            var mockVotaciones = new Mock<IGenericRepository<Votacion>>();
            mockVotaciones
                .Setup(r => r.GetAllWithIncludesAsync(
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>()))
                .ReturnsAsync(new List<Votacion> { votacion });

            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.Votaciones).Returns(mockVotaciones.Object);

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
            var categoria = new Categoria { Id = 1, Name = "IA", Evento = evento };
            var votacion = new Popular
            {
                Id = 1,
                FechaCierre = ahora.AddMinutes(-1),
                Estado = "Cerrada",
                NotificacionCierreEnviada = false,
                EnviarNotificacionApertura = false
            };
            votacion.Categoria = categoria;

            var mockVotaciones = new Mock<IGenericRepository<Votacion>>();
            mockVotaciones
                .Setup(r => r.GetAllWithIncludesAsync(
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>()))
                .ReturnsAsync(new List<Votacion> { votacion });

            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.Votaciones).Returns(mockVotaciones.Object);

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
            var categoria = new Categoria { Id = 1, Name = "IA", Evento = evento };
            var votacion = new Popular
            {
                Id = 1,
                FechaApertura = ahora.AddMinutes(-1),
                EnviarNotificacionApertura = true,
                NotificacionAperturaEnviada = true,
                Estado = "Pendiente"
            };
            votacion.Categoria = categoria;

            var mockVotaciones = new Mock<IGenericRepository<Votacion>>();
            mockVotaciones
                .Setup(r => r.GetAllWithIncludesAsync(
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>()))
                .ReturnsAsync(new List<Votacion> { votacion });

            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.Votaciones).Returns(mockVotaciones.Object);

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
            var categoria = new Categoria { Id = 1, Name = "IA", Evento = evento };
            var votacion = new Popular
            {
                Id = 1,
                FechaCierre = ahora.AddMinutes(3),
                Estado = "Abierta",
                NotificacionRecordatorioEnviada = true
            };
            votacion.Categoria = categoria;

            var mockVotaciones = new Mock<IGenericRepository<Votacion>>();
            mockVotaciones
                .Setup(r => r.GetAllWithIncludesAsync(
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>()))
                .ReturnsAsync(new List<Votacion> { votacion });

            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.Votaciones).Returns(mockVotaciones.Object);

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
            var categoria = new Categoria { Id = 1, Name = "IA", Evento = evento };
            var votacion = new Popular
            {
                Id = 1,
                FechaCierre = ahora.AddMinutes(-1),
                Estado = "Abierta",
                NotificacionCierreEnviada = true
            };
            votacion.Categoria = categoria;

            var mockVotaciones = new Mock<IGenericRepository<Votacion>>();
            mockVotaciones
                .Setup(r => r.GetAllWithIncludesAsync(
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>()))
                .ReturnsAsync(new List<Votacion> { votacion });

            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.Votaciones).Returns(mockVotaciones.Object);

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
            var mockVotaciones = new Mock<IGenericRepository<Votacion>>();
            mockVotaciones
                .Setup(r => r.GetAllWithIncludesAsync(
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>(),
                    It.IsAny<Expression<Func<Votacion, object>>>()))
                .ReturnsAsync(new List<Votacion>());

            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.Votaciones).Returns(mockVotaciones.Object);

            var mockSubject = new Mock<IVotacionStateSubject>();
            var detector = new VotacionStateCronDetector(mockUoW.Object, mockSubject.Object);

            await detector.DetectAndNotifyAsync();

            mockSubject.Verify(s => s.NotifyAsync(It.IsAny<VotacionStateChangedArgs>()), Times.Never);
            mockUoW.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
