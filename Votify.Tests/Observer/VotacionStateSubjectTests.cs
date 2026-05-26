using Microsoft.Extensions.DependencyInjection;
using Moq;
using Votify.Core.Enums;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Implementations;
using Votify.Services.Implementations.Observers;
using Xunit;

namespace Votify.Tests.Observer
{
    public class VotacionStateSubjectTests
    {
        private static IServiceProvider CreateMockServiceProvider(IEnumerable<IVotacionStateObserver> observers, RealTimeUINotificationObserver? uiObserver = null)
        {
            var mockScope = new Mock<IServiceScope>();
            mockScope.Setup(s => s.ServiceProvider.GetService(It.IsAny<Type>()))
                     .Returns((Type t) => t == typeof(IEnumerable<IVotacionStateObserver>) ? observers : null);

            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            mockScopeFactory.Setup(f => f.CreateScope())
                            .Returns(mockScope.Object);

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(It.IsAny<Type>()))
                               .Returns((Type t) =>
                               {
                                   if (t == typeof(IServiceScopeFactory)) return mockScopeFactory.Object;
                                   if (t == typeof(RealTimeUINotificationObserver)) return uiObserver ?? new RealTimeUINotificationObserver();
                                   return null;
                               });

            return mockServiceProvider.Object;
        }

        [Fact]
        public async Task NotifyAsync_CuandoSeNotifica_EjecutaTodosLosObservers()
        {
            // ARRANGE
            var mockObserver1 = new Mock<IVotacionStateObserver>();
            var mockObserver2 = new Mock<IVotacionStateObserver>();
            var mockObserver3 = new Mock<IVotacionStateObserver>();

            var observers = new List<IVotacionStateObserver>
            {
                mockObserver1.Object,
                mockObserver2.Object,
                mockObserver3.Object
            };

            var uiObserver = new RealTimeUINotificationObserver();
            var serviceProvider = CreateMockServiceProvider(observers, uiObserver);
            var subject = new VotacionStateSubject(serviceProvider, uiObserver);

            var args = new VotacionStateChangedArgs
            {
                EventType = VotacionStateEventType.Apertura,
                Votacion = new Popular { Id = 1 },
                Evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1)
            };

            // ACT
            await subject.NotifyAsync(args);

            // ASSERT
            mockObserver1.Verify(o => o.HandleAsync(args), Times.Once);
            mockObserver2.Verify(o => o.HandleAsync(args), Times.Once);
            mockObserver3.Verify(o => o.HandleAsync(args), Times.Once);
        }

        [Fact]
        public async Task NotifyAsync_CuandoUnObserverLanzaExcepcion_LosDemasSeSiguenEjecutando()
        {
            // ARRANGE
            var mockObserverOk1 = new Mock<IVotacionStateObserver>();
            var mockObserverFail = new Mock<IVotacionStateObserver>();
            var mockObserverOk2 = new Mock<IVotacionStateObserver>();

            mockObserverFail
                .Setup(o => o.HandleAsync(It.IsAny<VotacionStateChangedArgs>()))
                .ThrowsAsync(new InvalidOperationException("Error simulado"));

            var observers = new List<IVotacionStateObserver>
            {
                mockObserverOk1.Object,
                mockObserverFail.Object,
                mockObserverOk2.Object
            };

            var uiObserver = new RealTimeUINotificationObserver();
            var serviceProvider = CreateMockServiceProvider(observers, uiObserver);
            var subject = new VotacionStateSubject(serviceProvider, uiObserver);

            var args = new VotacionStateChangedArgs
            {
                EventType = VotacionStateEventType.Cierre,
                Votacion = new Popular { Id = 1 },
                Evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1)
            };

            // ACT & ASSERT
            await Assert.ThrowsAsync<InvalidOperationException>(() => subject.NotifyAsync(args));

            // Los observers que no fallaron sí se ejecutaron
            mockObserverOk1.Verify(o => o.HandleAsync(args), Times.Once);
            mockObserverOk2.Verify(o => o.HandleAsync(args), Times.Once);
        }

        [Fact]
        public void Subscribe_YUnsubscribe_SonNoOp_NoLanzanExcepcion()
        {
            // ARRANGE
            var uiObserver = new RealTimeUINotificationObserver();
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(RealTimeUINotificationObserver)))
                               .Returns(uiObserver);
            var subject = new VotacionStateSubject(mockServiceProvider.Object, uiObserver);
            var mockObserver = new Mock<IVotacionStateObserver>();

            // ACT & ASSERT
            var ex = Record.Exception(() => subject.Subscribe(mockObserver.Object));
            Assert.Null(ex);

            ex = Record.Exception(() => subject.Unsubscribe(mockObserver.Object));
            Assert.Null(ex);
        }
    }
}
