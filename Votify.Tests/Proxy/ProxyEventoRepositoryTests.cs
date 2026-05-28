using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Persistence.Repositories;
using Xunit;

namespace Votify.Tests.Proxy
{
    public class ProxyEventoRepositoryTests
    {
        private IMemoryCache CreateCache() => new MemoryCache(new MemoryCacheOptions());

        private Mock<IEventoRepository> CreateMockInner(Evento? evento = null)
        {
            var mock = new Mock<IEventoRepository>();
            mock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(evento);
            mock.Setup(r => r.GetAllAsync()).ReturnsAsync(evento != null ? new List<Evento> { evento } : new List<Evento>());
            mock.Setup(r => r.ObtenerEventoConDetallesAsync(It.IsAny<int>())).ReturnsAsync(evento);
            mock.Setup(r => r.ObtenerEventosDisponiblesAsync()).ReturnsAsync(evento != null ? new List<Evento> { evento } : new List<Evento>());
            mock.Setup(r => r.AddAsync(It.IsAny<Evento>())).ReturnsAsync((Evento e) => e);
            mock.Setup(r => r.UpdateAsync(It.IsAny<Evento>())).Returns(Task.CompletedTask);
            mock.Setup(r => r.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
            return mock;
        }

        [Fact]
        public async Task GetByIdAsync_PrimeraLlama_DelegaEnRealSubject()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1) { Id = 1 };
            var mockRealSubject = CreateMockInner(evento);
            var cache = CreateCache();
            var proxy = new ProxyEventoRepository(mockRealSubject.Object, cache);

            var resultado = await proxy.GetByIdAsync(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            mockRealSubject.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_SegundaLlama_UsaCache()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1) { Id = 1 };
            var mockRealSubject = CreateMockInner(evento);
            var cache = CreateCache();
            var proxy = new ProxyEventoRepository(mockRealSubject.Object, cache);

            var resultado1 = await proxy.GetByIdAsync(1);
            var resultado2 = await proxy.GetByIdAsync(1);

            Assert.Equal(1, resultado1.Id);
            Assert.Equal(1, resultado2.Id);
            mockRealSubject.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_PrimeraLlama_DelegaEnRealSubject()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1) { Id = 1 };
            var mockRealSubject = CreateMockInner(evento);
            var cache = CreateCache();
            var proxy = new ProxyEventoRepository(mockRealSubject.Object, cache);

            var resultado = await proxy.GetAllAsync();

            Assert.Single(resultado);
            mockRealSubject.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_SegundaLlama_UsaCache()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1) { Id = 1 };
            var mockRealSubject = CreateMockInner(evento);
            var cache = CreateCache();
            var proxy = new ProxyEventoRepository(mockRealSubject.Object, cache);

            var resultado1 = await proxy.GetAllAsync();
            var resultado2 = await proxy.GetAllAsync();

            Assert.Single(resultado1);
            Assert.Single(resultado2);
            mockRealSubject.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task ObtenerEventoConDetallesAsync_PrimeraLlama_DelegaEnRealSubject()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1) { Id = 1 };
            var mockRealSubject = CreateMockInner(evento);
            var cache = CreateCache();
            var proxy = new ProxyEventoRepository(mockRealSubject.Object, cache);

            var resultado = await proxy.ObtenerEventoConDetallesAsync(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            mockRealSubject.Verify(r => r.ObtenerEventoConDetallesAsync(1), Times.Once);
        }

        [Fact]
        public async Task ObtenerEventoConDetallesAsync_SegundaLlama_UsaCache()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1) { Id = 1 };
            var mockRealSubject = CreateMockInner(evento);
            var cache = CreateCache();
            var proxy = new ProxyEventoRepository(mockRealSubject.Object, cache);

            var resultado1 = await proxy.ObtenerEventoConDetallesAsync(1);
            var resultado2 = await proxy.ObtenerEventoConDetallesAsync(1);

            Assert.Equal(1, resultado1.Id);
            Assert.Equal(1, resultado2.Id);
            mockRealSubject.Verify(r => r.ObtenerEventoConDetallesAsync(1), Times.Once);
        }

        [Fact]
        public async Task ObtenerEventosDisponiblesAsync_PrimeraLlama_DelegaEnRealSubject()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1) { Id = 1 };
            var mockRealSubject = CreateMockInner(evento);
            var cache = CreateCache();
            var proxy = new ProxyEventoRepository(mockRealSubject.Object, cache);

            var resultado = await proxy.ObtenerEventosDisponiblesAsync();

            Assert.Single(resultado);
            mockRealSubject.Verify(r => r.ObtenerEventosDisponiblesAsync(), Times.Once);
        }

        [Fact]
        public async Task ObtenerEventosDisponiblesAsync_SegundaLlama_UsaCache()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1) { Id = 1 };
            var mockRealSubject = CreateMockInner(evento);
            var cache = CreateCache();
            var proxy = new ProxyEventoRepository(mockRealSubject.Object, cache);

            var resultado1 = await proxy.ObtenerEventosDisponiblesAsync();
            var resultado2 = await proxy.ObtenerEventosDisponiblesAsync();

            Assert.Single(resultado1);
            Assert.Single(resultado2);
            mockRealSubject.Verify(r => r.ObtenerEventosDisponiblesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddAsync_InvalidaCache()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1) { Id = 1 };
            var mockRealSubject = CreateMockInner(evento);
            var cache = CreateCache();
            var proxy = new ProxyEventoRepository(mockRealSubject.Object, cache);

            await proxy.GetAllAsync();
            await proxy.AddAsync(evento);
            await proxy.GetAllAsync();

            mockRealSubject.Verify(r => r.GetAllAsync(), Times.Exactly(2));
        }

        [Fact]
        public async Task UpdateAsync_InvalidaCache()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1) { Id = 1 };
            var mockRealSubject = CreateMockInner(evento);
            var cache = CreateCache();
            var proxy = new ProxyEventoRepository(mockRealSubject.Object, cache);

            await proxy.GetAllAsync();
            await proxy.UpdateAsync(evento);
            await proxy.GetAllAsync();

            mockRealSubject.Verify(r => r.GetAllAsync(), Times.Exactly(2));
        }

        [Fact]
        public async Task DeleteAsync_InvalidaCache()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1) { Id = 1 };
            var mockRealSubject = CreateMockInner(evento);
            var cache = CreateCache();
            var proxy = new ProxyEventoRepository(mockRealSubject.Object, cache);

            await proxy.GetAllAsync();
            await proxy.DeleteAsync(1);
            await proxy.GetAllAsync();

            mockRealSubject.Verify(r => r.GetAllAsync(), Times.Exactly(2));
        }
    }
}
