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
    public class CachedGenericRepositoryTests
    {
        private IMemoryCache CreateCache() => new MemoryCache(new MemoryCacheOptions());

        [Fact]
        public async Task GetByIdAsync_PrimeraLlama_DelegaEnInner()
        {
            var mockInner = new Mock<IGenericRepository<Juez>>();
            var juez = new Juez { Id = 1, Name = "Juez1" };
            mockInner.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(juez);

            var cache = CreateCache();
            var cachedRepo = new CachedGenericRepository<Juez>(mockInner.Object, cache);

            var resultado = await cachedRepo.GetByIdAsync(1);

            Assert.NotNull(resultado);
            Assert.Equal("Juez1", resultado.Name);
            mockInner.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_SegundaLlama_UsaCache()
        {
            var mockInner = new Mock<IGenericRepository<Juez>>();
            var juez = new Juez { Id = 1, Name = "Juez1" };
            mockInner.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(juez);

            var cache = CreateCache();
            var cachedRepo = new CachedGenericRepository<Juez>(mockInner.Object, cache);

            var resultado1 = await cachedRepo.GetByIdAsync(1);
            var resultado2 = await cachedRepo.GetByIdAsync(1);

            Assert.Equal("Juez1", resultado1.Name);
            Assert.Equal("Juez1", resultado2.Name);
            mockInner.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_CuandoNoExiste_NoCachearNull()
        {
            var mockInner = new Mock<IGenericRepository<Juez>>();
            mockInner.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Juez?)null);

            var cache = CreateCache();
            var cachedRepo = new CachedGenericRepository<Juez>(mockInner.Object, cache);

            var resultado1 = await cachedRepo.GetByIdAsync(999);
            var resultado2 = await cachedRepo.GetByIdAsync(999);

            Assert.Null(resultado1);
            Assert.Null(resultado2);
            mockInner.Verify(r => r.GetByIdAsync(999), Times.Exactly(2));
        }

        [Fact]
        public async Task GetAllAsync_PrimeraLlama_DelegaEnInner()
        {
            var mockInner = new Mock<IGenericRepository<Juez>>();
            var jueces = new List<Juez>
            {
                new Juez { Id = 1, Name = "Juez1" },
                new Juez { Id = 2, Name = "Juez2" }
            };
            mockInner.Setup(r => r.GetAllAsync()).ReturnsAsync(jueces);

            var cache = CreateCache();
            var cachedRepo = new CachedGenericRepository<Juez>(mockInner.Object, cache);

            var resultado = await cachedRepo.GetAllAsync();

            Assert.Equal(2, resultado.Count());
            mockInner.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_SegundaLlama_UsaCache()
        {
            var mockInner = new Mock<IGenericRepository<Juez>>();
            var jueces = new List<Juez>
            {
                new Juez { Id = 1, Name = "Juez1" },
                new Juez { Id = 2, Name = "Juez2" }
            };
            mockInner.Setup(r => r.GetAllAsync()).ReturnsAsync(jueces);

            var cache = CreateCache();
            var cachedRepo = new CachedGenericRepository<Juez>(mockInner.Object, cache);

            var resultado1 = await cachedRepo.GetAllAsync();
            var resultado2 = await cachedRepo.GetAllAsync();

            Assert.Equal(2, resultado1.Count());
            Assert.Equal(2, resultado2.Count());
            mockInner.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task AddAsync_NoInvalidaCacheAutomaticamente()
        {
            var mockInner = new Mock<IGenericRepository<Juez>>();
            var juez = new Juez { Id = 1, Name = "Juez1" };
            mockInner.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Juez> { juez });
            mockInner.Setup(r => r.AddAsync(It.IsAny<Juez>())).ReturnsAsync((Juez j) => j);

            var cache = CreateCache();
            var cachedRepo = new CachedGenericRepository<Juez>(mockInner.Object, cache);

            await cachedRepo.GetAllAsync();
            await cachedRepo.AddAsync(new Juez { Id = 2, Name = "Juez2" });
            await cachedRepo.GetAllAsync();

            mockInner.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_NoInvalidaCacheAutomaticamente()
        {
            var mockInner = new Mock<IGenericRepository<Juez>>();
            var juez = new Juez { Id = 1, Name = "Juez1" };
            mockInner.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Juez> { juez });
            mockInner.Setup(r => r.UpdateAsync(It.IsAny<Juez>())).Returns(Task.CompletedTask);

            var cache = CreateCache();
            var cachedRepo = new CachedGenericRepository<Juez>(mockInner.Object, cache);

            await cachedRepo.GetAllAsync();
            await cachedRepo.UpdateAsync(juez);
            await cachedRepo.GetAllAsync();

            mockInner.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_NoInvalidaCacheAutomaticamente()
        {
            var mockInner = new Mock<IGenericRepository<Juez>>();
            var juez = new Juez { Id = 1, Name = "Juez1" };
            mockInner.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Juez> { juez });
            mockInner.Setup(r => r.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

            var cache = CreateCache();
            var cachedRepo = new CachedGenericRepository<Juez>(mockInner.Object, cache);

            await cachedRepo.GetAllAsync();
            await cachedRepo.DeleteAsync(1);
            await cachedRepo.GetAllAsync();

            mockInner.Verify(r => r.GetAllAsync(), Times.Once);
        }
    }
}
