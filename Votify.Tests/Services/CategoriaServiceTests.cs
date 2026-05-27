using Moq;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Core.Enums;
using Votify.Services.Implementations;
using Votify.Services.Models.Requests;
using Xunit;

namespace Votify.Tests.Services
{
    public class CategoriaServiceTests
    {
        private Mock<IUnitOfWork> CreateUnitOfWorkMock(Categoria? categoria = null)
        {
            var mockCategorias = new Mock<IGenericRepository<Categoria>>();
            mockCategorias.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(categoria);

            var mockCategoriaRepo = new Mock<ICategoriaRepository>();
            mockCategoriaRepo.Setup(r => r.ObtenerCategoriaConVotacionYVotosAsync(It.IsAny<int>())).ReturnsAsync(categoria);

            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.Setup(u => u.Categorias).Returns(mockCategorias.Object);
            mockUoW.Setup(u => u.CategoriaRepository).Returns(mockCategoriaRepo.Object);

            return mockUoW;
        }

        [Fact]
        public async Task ObtenerPorIdAsync_CuandoExiste_DevuelveCategoria()
        {
            var categoria = new Categoria { Id = 1, Name = "Test" };
            var mockUoW = CreateUnitOfWorkMock(categoria);
            var service = new CategoriaService(mockUoW.Object);

            var resultado = await service.ObtenerPorIdAsync(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            Assert.Equal("Test", resultado.Name);
        }

        [Fact]
        public async Task ObtenerPorIdAsync_CuandoNoExiste_DevuelveNull()
        {
            var mockUoW = CreateUnitOfWorkMock(categoria: null);
            var service = new CategoriaService(mockUoW.Object);

            var resultado = await service.ObtenerPorIdAsync(999);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task CrearAsync_AgregaNuevaCategoria()
        {
            var mockUoW = CreateUnitOfWorkMock();
            var service = new CategoriaService(mockUoW.Object);

            var categoria = new Categoria { Id = 1, Name = "Nueva Categoria" };
            await service.CrearAsync(categoria);

            mockUoW.Verify(u => u.Categorias.AddAsync(categoria), Times.Once);
            mockUoW.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ActualizaCategoria()
        {
            var mockUoW = CreateUnitOfWorkMock();
            var service = new CategoriaService(mockUoW.Object);

            var categoria = new Categoria { Id = 1, Name = "Categoria Actualizada" };
            await service.UpdateAsync(categoria);

            mockUoW.Verify(u => u.Categorias.UpdateAsync(categoria), Times.Once);
            mockUoW.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_CuandoExiste_EliminaCategoria()
        {
            var categoria = new Categoria { Id = 1, Name = "Test" };
            var mockUoW = CreateUnitOfWorkMock(categoria);
            var service = new CategoriaService(mockUoW.Object);

            await service.DeleteAsync(1);

            mockUoW.Verify(u => u.Categorias.DeleteAsync(1), Times.Once);
            mockUoW.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_CuandoNoExiste_NoHaceNada()
        {
            var mockUoW = CreateUnitOfWorkMock(categoria: null);
            var service = new CategoriaService(mockUoW.Object);

            await service.DeleteAsync(999);

            mockUoW.Verify(u => u.Categorias.DeleteAsync(It.IsAny<int>()), Times.Never);
            mockUoW.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AgregarPremioAsync_CuandoCategoriaExiste_AgregaPremio()
        {
            var categoria = new Categoria { Id = 1, Name = "Test" };
            var mockUoW = CreateUnitOfWorkMock(categoria);
            var service = new CategoriaService(mockUoW.Object);

            var request = new AgregarPremioRequest
            {
                categoriaID = 1,
                nombrePremio = "Primer Lugar",
                premioDesc = "Descripción",
                puesto = 1,
                PermiteEmpate = false
            };

            await service.AgregarPremioAsync(request);

            Assert.Single(categoria.Premios);
            Assert.Equal("Primer Lugar", categoria.Premios[0].Name);
            mockUoW.Verify(u => u.Categorias.UpdateAsync(categoria), Times.Once);
            mockUoW.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AgregarPremioAsync_CuandoCategoriaNoExiste_NoHaceNada()
        {
            var mockUoW = CreateUnitOfWorkMock(categoria: null);
            var service = new CategoriaService(mockUoW.Object);

            var request = new AgregarPremioRequest
            {
                categoriaID = 999,
                nombrePremio = "Primer Lugar",
                premioDesc = "Descripción",
                puesto = 1,
                PermiteEmpate = false
            };

            await service.AgregarPremioAsync(request);

            mockUoW.Verify(u => u.Categorias.UpdateAsync(It.IsAny<Categoria>()), Times.Never);
            mockUoW.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task EliminarPremioAsync_CuandoCategoriaNoExiste_LanzaExcepcion()
        {
            var mockUoW = CreateUnitOfWorkMock(categoria: null);
            var service = new CategoriaService(mockUoW.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.EliminarPremioAsync(999, 1));
        }

        [Fact]
        public async Task CerrarVotacionAsync_CuandoCategoriaSinVotacion_LanzaExcepcion()
        {
            var categoria = new Categoria { Id = 1, Name = "Test" };
            var mockUoW = CreateUnitOfWorkMock(categoria);
            var service = new CategoriaService(mockUoW.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.CerrarVotacionAsync(1));
        }

        [Fact]
        public async Task CerrarVotacionAsync_CuandoVotacionExiste_CierraVotacion()
        {
            var categoria = new Categoria { Id = 1, Name = "Test" };
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Abierta };
            categoria.Votacion = votacion;

            var mockUoW = CreateUnitOfWorkMock(categoria);
            var service = new CategoriaService(mockUoW.Object);

            await service.CerrarVotacionAsync(1);

            Assert.True(votacion.EstaCerrada);
            Assert.Equal(EstadoVotacion.Cerrada, votacion.Estado);
            mockUoW.Verify(u => u.Categorias.UpdateAsync(categoria), Times.Once);
            mockUoW.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ForzarApertura_CuandoVotacionExiste_AbreVotacion()
        {
            var categoria = new Categoria { Id = 1, Name = "Test" };
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Programada };
            categoria.Votacion = votacion;

            var mockUoW = CreateUnitOfWorkMock(categoria);
            var service = new CategoriaService(mockUoW.Object);

            await service.ForzarApertura(categoria);

            Assert.Equal(EstadoVotacion.Abierta, votacion.Estado);
            mockUoW.Verify(u => u.Categorias.UpdateAsync(categoria), Times.Once);
            mockUoW.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ForzarCierre_CuandoVotacionExiste_CierraVotacion()
        {
            var categoria = new Categoria { Id = 1, Name = "Test" };
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Abierta };
            categoria.Votacion = votacion;

            var mockUoW = CreateUnitOfWorkMock(categoria);
            var service = new CategoriaService(mockUoW.Object);

            await service.ForzarCierre(categoria);

            Assert.True(votacion.EstaCerrada);
            Assert.Equal(EstadoVotacion.Cerrada, votacion.Estado);
            mockUoW.Verify(u => u.Categorias.UpdateAsync(categoria), Times.Once);
            mockUoW.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task PausarVotacion_CuandoVotacionExiste_PausaVotacion()
        {
            var categoria = new Categoria { Id = 1, Name = "Test" };
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Abierta };
            categoria.Votacion = votacion;

            var mockUoW = CreateUnitOfWorkMock(categoria);
            var service = new CategoriaService(mockUoW.Object);

            await service.PausarVotacion(categoria);

            Assert.Equal(EstadoVotacion.Pausada, votacion.Estado);
            mockUoW.Verify(u => u.Categorias.UpdateAsync(categoria), Times.Once);
            mockUoW.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ForzarProgramada_CuandoVotacionExiste_ProgramaVotacion()
        {
            var categoria = new Categoria { Id = 1, Name = "Test" };
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Pausada };
            categoria.Votacion = votacion;

            var mockUoW = CreateUnitOfWorkMock(categoria);
            var service = new CategoriaService(mockUoW.Object);

            await service.ForzarProgramada(categoria);

            Assert.Equal(EstadoVotacion.Programada, votacion.Estado);
            mockUoW.Verify(u => u.Categorias.UpdateAsync(categoria), Times.Once);
            mockUoW.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
