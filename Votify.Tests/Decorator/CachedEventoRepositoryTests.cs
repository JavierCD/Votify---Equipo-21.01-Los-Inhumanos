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

namespace Votify.Tests.Decorator
{
    public class CachedEventoRepositoryTests
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
        public async Task GetByIdAsync_PrimeraLlama_DelegaEnInner()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1) { Id = 1 };
            var mockInner = CreateMockInner(evento);
            var cache = CreateCache();
            var cachedRepo = new CachedEventoRepository(mockInner.Object, cache);

            var resultado = await cachedRepo.GetByIdAsync(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            mockInner.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_SegundaLlama_UsaCache()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1) { Id = 1 };
            var mockInner = CreateMockInner(evento);
            var cache = CreateCache();
            var cachedRepo = new CachedEventoRepository(mockInner.Object, cache);

            var resultado1 = await cachedRepo.GetByIdAsync(1);
            var resultado2 = await cachedRepo.GetByIdAsync(1);

            Assert.Equal(1, resultado1.Id);
            Assert.Equal(1, resultado2.Id);
            mockInner.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_PrimeraLlama_DelegaEnInner()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1) { Id = 1 };
            var mockInner = CreateMockInner(evento);
            var cache = CreateCache();
            var cachedRepo = new CachedEventoRepository(mockInner.Object, cache);

            var resultado = await cachedRepo.GetAllAsync();

            Assert.Single(resultado);
            mockInner.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_SegundaLlama_UsaCache()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1) { Id = 1 };
            var mockInner = CreateMockInner(evento);
            var cache = CreateCache();
            var cachedRepo = new CachedEventoRepository(mockInner.Object, cache);

            var resultado1 = await cachedRepo.GetAllAsync();
            var resultado2 = await cachedRepo.GetAllAsync();

            Assert.Single(resultado1);
            Assert.Single(resultado2);
            mockInner.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task ObtenerEventoConDetallesAsync_PrimeraLlama_DelegaEnInner()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1) { Id = 1 };
            var mockInner = CreateMockInner(evento);
            var cache = CreateCache();
            var cachedRepo = new CachedEventoRepository(mockInner.Object, cache);

            var resultado = await cachedRepo.ObtenerEventoConDetallesAsync(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            mockInner.Verify(r => r.ObtenerEventoConDetallesAsync(1), Times.Once);
        }

        [Fact]
        public async Task ObtenerEventoConDetallesAsync_SegundaLlama_UsaCache()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1) { Id = 1 };
            var mockInner = CreateMockInner(evento);
            var cache = CreateCache();
            var cachedRepo = new CachedEventoRepository(mockInner.Object, cache);

            var resultado1 = await cachedRepo.ObtenerEventoConDetallesAsync(1);
            var resultado2 = await cachedRepo.ObtenerEventoConDetallesAsync(1);

            Assert.Equal(1, resultado1.Id);
            Assert.Equal(1, resultado2.Id);
            mockInner.Verify(r => r.ObtenerEventoConDetallesAsync(1), Times.Once);
        }

        [Fact]
        public async Task ObtenerEventosDisponiblesAsync_PrimeraLlama_DelegaEnInner()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1) { Id = 1 };
            var mockInner = CreateMockInner(evento);
            var cache = CreateCache();
            var cachedRepo = new CachedEventoRepository(mockInner.Object, cache);

            var resultado = await cachedRepo.ObtenerEventosDisponiblesAsync();

            Assert.Single(resultado);
            mockInner.Verify(r => r.ObtenerEventosDisponiblesAsync(), Times.Once);
        }

        [Fact]
        public async Task ObtenerEventosDisponiblesAsync_SegundaLlama_UsaCache()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1) { Id = 1 };
            var mockInner = CreateMockInner(evento);
            var cache = CreateCache();
            var cachedRepo = new CachedEventoRepository(mockInner.Object, cache);

            var resultado1 = await cachedRepo.ObtenerEventosDisponiblesAsync();
            var resultado2 = await cachedRepo.ObtenerEventosDisponiblesAsync();

            Assert.Single(resultado1);
            Assert.Single(resultado2);
            mockInner.Verify(r => r.ObtenerEventosDisponiblesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddAsync_NoInvalidaCacheAutomaticamente()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1) { Id = 1 };
            var mockInner = CreateMockInner(evento);
            var cache = CreateCache();
            var cachedRepo = new CachedEventoRepository(mockInner.Object, cache);

            await cachedRepo.GetAllAsync();
            await cachedRepo.AddAsync(evento);
            await cachedRepo.GetAllAsync();

            mockInner.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_NoInvalidaCacheAutomaticamente()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1) { Id = 1 };
            var mockInner = CreateMockInner(evento);
            var cache = CreateCache();
            var cachedRepo = new CachedEventoRepository(mockInner.Object, cache);

            await cachedRepo.GetAllAsync();
            await cachedRepo.UpdateAsync(evento);
            await cachedRepo.GetAllAsync();

            mockInner.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_NoInvalidaCacheAutomaticamente()
        {
            var evento = new HackathonEvent("Hackathon", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1) { Id = 1 };
            var mockInner = CreateMockInner(evento);
            var cache = CreateCache();
            var cachedRepo = new CachedEventoRepository(mockInner.Object, cache);

            await cachedRepo.GetAllAsync();
            await cachedRepo.DeleteAsync(1);
            await cachedRepo.GetAllAsync();

            mockInner.Verify(r => r.GetAllAsync(), Times.Once);
        }
    }
}
