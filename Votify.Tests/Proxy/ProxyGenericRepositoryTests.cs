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
    public class ProxyGenericRepositoryTests
    {
        private IMemoryCache CreateCache() => new MemoryCache(new MemoryCacheOptions());

        [Fact]
        public async Task GetByIdAsync_PrimeraLlama_DelegaEnRealSubject()
        {
            var mockRealSubject = new Mock<IGenericRepository<Juez>>();
            var juez = new Juez { Id = 1, Name = "Juez1" };
            mockRealSubject.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(juez);

            var cache = CreateCache();
            var proxy = new ProxyGenericRepository<Juez>(mockRealSubject.Object, cache);

            var resultado = await proxy.GetByIdAsync(1);

            Assert.NotNull(resultado);
            Assert.Equal("Juez1", resultado.Name);
            mockRealSubject.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_SegundaLlama_UsaCache()
        {
            var mockRealSubject = new Mock<IGenericRepository<Juez>>();
            var juez = new Juez { Id = 1, Name = "Juez1" };
            mockRealSubject.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(juez);

            var cache = CreateCache();
            var proxy = new ProxyGenericRepository<Juez>(mockRealSubject.Object, cache);

            var resultado1 = await proxy.GetByIdAsync(1);
            var resultado2 = await proxy.GetByIdAsync(1);

            Assert.Equal("Juez1", resultado1.Name);
            Assert.Equal("Juez1", resultado2.Name);
            mockRealSubject.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_CuandoNoExiste_NoCachearNull()
        {
            var mockRealSubject = new Mock<IGenericRepository<Juez>>();
            mockRealSubject.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Juez?)null);

            var cache = CreateCache();
            var proxy = new ProxyGenericRepository<Juez>(mockRealSubject.Object, cache);

            var resultado1 = await proxy.GetByIdAsync(999);
            var resultado2 = await proxy.GetByIdAsync(999);

            Assert.Null(resultado1);
            Assert.Null(resultado2);
            mockRealSubject.Verify(r => r.GetByIdAsync(999), Times.Exactly(2));
        }

        [Fact]
        public async Task GetAllAsync_PrimeraLlama_DelegaEnRealSubject()
        {
            var mockRealSubject = new Mock<IGenericRepository<Juez>>();
            var jueces = new List<Juez>
            {
                new Juez { Id = 1, Name = "Juez1" },
                new Juez { Id = 2, Name = "Juez2" }
            };
            mockRealSubject.Setup(r => r.GetAllAsync()).ReturnsAsync(jueces);

            var cache = CreateCache();
            var proxy = new ProxyGenericRepository<Juez>(mockRealSubject.Object, cache);

            var resultado = await proxy.GetAllAsync();

            Assert.Equal(2, resultado.Count());
            mockRealSubject.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_SegundaLlama_UsaCache()
        {
            var mockRealSubject = new Mock<IGenericRepository<Juez>>();
            var jueces = new List<Juez>
            {
                new Juez { Id = 1, Name = "Juez1" },
                new Juez { Id = 2, Name = "Juez2" }
            };
            mockRealSubject.Setup(r => r.GetAllAsync()).ReturnsAsync(jueces);

            var cache = CreateCache();
            var proxy = new ProxyGenericRepository<Juez>(mockRealSubject.Object, cache);

            var resultado1 = await proxy.GetAllAsync();
            var resultado2 = await proxy.GetAllAsync();

            Assert.Equal(2, resultado1.Count());
            Assert.Equal(2, resultado2.Count());
            mockRealSubject.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task AddAsync_InvalidaCache()
        {
            var mockRealSubject = new Mock<IGenericRepository<Juez>>();
            var juez = new Juez { Id = 1, Name = "Juez1" };
            mockRealSubject.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Juez> { juez });
            mockRealSubject.Setup(r => r.AddAsync(It.IsAny<Juez>())).ReturnsAsync((Juez j) => j);

            var cache = CreateCache();
            var proxy = new ProxyGenericRepository<Juez>(mockRealSubject.Object, cache);

            await proxy.GetAllAsync();
            await proxy.AddAsync(new Juez { Id = 2, Name = "Juez2" });
            await proxy.GetAllAsync();

            mockRealSubject.Verify(r => r.GetAllAsync(), Times.Exactly(2));
        }

        [Fact]
        public async Task UpdateAsync_InvalidaCache()
        {
            var mockRealSubject = new Mock<IGenericRepository<Juez>>();
            var juez = new Juez { Id = 1, Name = "Juez1" };
            mockRealSubject.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Juez> { juez });
            mockRealSubject.Setup(r => r.UpdateAsync(It.IsAny<Juez>())).Returns(Task.CompletedTask);

            var cache = CreateCache();
            var proxy = new ProxyGenericRepository<Juez>(mockRealSubject.Object, cache);

            await proxy.GetAllAsync();
            await proxy.UpdateAsync(juez);
            await proxy.GetAllAsync();

            mockRealSubject.Verify(r => r.GetAllAsync(), Times.Exactly(2));
        }

        [Fact]
        public async Task DeleteAsync_InvalidaCache()
        {
            var mockRealSubject = new Mock<IGenericRepository<Juez>>();
            var juez = new Juez { Id = 1, Name = "Juez1" };
            mockRealSubject.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Juez> { juez });
            mockRealSubject.Setup(r => r.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

            var cache = CreateCache();
            var proxy = new ProxyGenericRepository<Juez>(mockRealSubject.Object, cache);

            await proxy.GetAllAsync();
            await proxy.DeleteAsync(1);
            await proxy.GetAllAsync();

            mockRealSubject.Verify(r => r.GetAllAsync(), Times.Exactly(2));
        }
    }
}
