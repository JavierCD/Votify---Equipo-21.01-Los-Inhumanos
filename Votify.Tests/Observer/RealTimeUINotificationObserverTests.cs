using Votify.Core.Enums;
using Votify.Core.Models;
using Votify.Services.Implementations.Observers;
using Xunit;

namespace Votify.Tests.Observer
{
    public class RealTimeUINotificationObserverTests
    {
        [Fact]
        public async Task HandleAsync_CuandoEventoEsCierre_DisparaEventoOnVotacionCerrada()
        {
            var votacion = new Popular { Id = 42 };
            var args = new VotacionStateChangedArgs
            {
                EventType = VotacionStateEventType.Cierre,
                Votacion = votacion,
                Evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1)
            };

            var observer = new RealTimeUINotificationObserver();
            int? capturedId = null;
            observer.OnVotacionCerrada += (id) => capturedId = id;

            await observer.HandleAsync(args);

            Assert.Equal(42, capturedId);
        }

        [Fact]
        public async Task HandleAsync_CuandoEventoEsApertura_NoDisparaEvento()
        {
            var votacion = new Popular { Id = 1 };
            var args = new VotacionStateChangedArgs
            {
                EventType = VotacionStateEventType.Apertura,
                Votacion = votacion,
                Evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1)
            };

            var observer = new RealTimeUINotificationObserver();
            bool eventFired = false;
            observer.OnVotacionCerrada += (id) => eventFired = true;

            await observer.HandleAsync(args);

            Assert.False(eventFired);
        }

        [Fact]
        public async Task HandleAsync_CuandoEventoEsProximoCierre_NoDisparaEvento()
        {
            var votacion = new Popular { Id = 1 };
            var args = new VotacionStateChangedArgs
            {
                EventType = VotacionStateEventType.ProximoCierre,
                Votacion = votacion,
                Evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1)
            };

            var observer = new RealTimeUINotificationObserver();
            bool eventFired = false;
            observer.OnVotacionCerrada += (id) => eventFired = true;

            await observer.HandleAsync(args);

            Assert.False(eventFired);
        }

        [Fact]
        public async Task HandleAsync_CuandoNoHaySuscriptores_NoLanzaExcepcion()
        {
            var votacion = new Popular { Id = 1 };
            var args = new VotacionStateChangedArgs
            {
                EventType = VotacionStateEventType.Cierre,
                Votacion = votacion,
                Evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1)
            };

            var observer = new RealTimeUINotificationObserver();

            var ex = await Record.ExceptionAsync(() => observer.HandleAsync(args));

            Assert.Null(ex);
        }
    }
}
