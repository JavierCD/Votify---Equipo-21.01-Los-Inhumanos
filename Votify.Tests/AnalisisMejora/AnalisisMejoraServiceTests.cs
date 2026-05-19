using Moq;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Implementations.Analysis;
using Xunit;

namespace Votify.Tests.AnalisisMejora
{
    public class AnalisisMejoraServiceTests
    {
        private static VotoExperto CreateVotoExperto(int id, int proyectoId, string? comentario, string? juezName = null)
        {
            var juez = string.IsNullOrEmpty(juezName) ? null : new Juez { Id = proyectoId, Name = juezName };
            var proyecto = new AiProject($"Proyecto{proyectoId}", 1);
            proyecto.Id = proyectoId;
            return new VotoExperto(1, proyectoId, 5.0, false, null, comentario)
            {
                Id = id,
                Juez = juez,
                Proyecto = proyecto
            };
        }

        private static Mock<IUnitOfWork> CreateUnitOfWorkMock(List<VotoExperto> comentarios)
        {
            var mockRepo = new Mock<IVotoExpertoRepository>();
            mockRepo.Setup(r => r.ObtenerComentariosPorProyectoAsync(It.IsAny<int>()))
                    .ReturnsAsync(comentarios);

            var mockUow = new Mock<IUnitOfWork>();
            mockUow.Setup(u => u.VotoExpertoRepository).Returns(mockRepo.Object);

            return mockUow;
        }

        [Fact]
        public async Task CuandoHayComentarios_GeneraHojaRutaConSugerencias()
        {
            var comentarios = new List<VotoExperto>
            {
                CreateVotoExperto(1, 1, "Buen código pero falta documentación", "Juez1"),
                CreateVotoExperto(2, 1, "Mejorar los tests unitarios", "Juez2")
            };

            var mockUow = CreateUnitOfWorkMock(comentarios);

            var mockIA = new Mock<IIAProvider>();
            mockIA.Setup(p => p.AnalizarAsync(It.IsAny<string>()))
                  .ReturnsAsync("""
                    {
                        "sugerencias": [
                            {
                                "prioridad": 1,
                                "categoria": "Documentación",
                                "descripcion": "Falta documentación",
                                "accionRecomendada": "Añadir comentarios XML"
                            }
                        ]
                    }
                    """);

            var service = new AnalisisMejoraService(mockUow.Object, mockIA.Object);

            var resultado = await service.GenerarHojaRutaAsync(1);

            Assert.Equal(1, resultado.ProyectoId);
            Assert.Equal("Proyecto1", resultado.ProyectoNombre);
            Assert.Equal(2, resultado.TotalComentariosAnalizados);
            Assert.Single(resultado.Sugerencias);
            Assert.Equal(1, resultado.Sugerencias[0].Prioridad);
            Assert.Equal("Documentación", resultado.Sugerencias[0].Categoria);
        }

        [Fact]
        public async Task CuandoNoHayComentarios_DevuelveHojaRutaVaciaSinLlamarIA()
        {
            var mockUow = CreateUnitOfWorkMock(new List<VotoExperto>());

            var mockIA = new Mock<IIAProvider>();

            var service = new AnalisisMejoraService(mockUow.Object, mockIA.Object);

            var resultado = await service.GenerarHojaRutaAsync(1);

            Assert.Equal(1, resultado.ProyectoId);
            Assert.Equal(0, resultado.TotalComentariosAnalizados);
            Assert.Empty(resultado.Sugerencias);
            mockIA.Verify(p => p.AnalizarAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task CuandoIAFalla_LanzaExcepcion()
        {
            var comentarios = new List<VotoExperto>
            {
                CreateVotoExperto(1, 1, "Comentario válido", "Juez1")
            };

            var mockUow = CreateUnitOfWorkMock(comentarios);

            var mockIA = new Mock<IIAProvider>();
            mockIA.Setup(p => p.AnalizarAsync(It.IsAny<string>()))
                  .ThrowsAsync(new InvalidOperationException("Error de IA"));

            var service = new AnalisisMejoraService(mockUow.Object, mockIA.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GenerarHojaRutaAsync(1));
        }

        [Fact]
        public async Task FiltraComentariosVaciosONull()
        {
            var comentarios = new List<VotoExperto>
            {
                CreateVotoExperto(1, 1, "Comentario válido", "Juez1")
            };

            var mockUow = CreateUnitOfWorkMock(comentarios);

            string? promptCapturado = null;
            var mockIA = new Mock<IIAProvider>();
            mockIA.Setup(p => p.AnalizarAsync(It.IsAny<string>()))
                  .Callback<string>(p => promptCapturado = p)
                  .ReturnsAsync("{ \"sugerencias\": [] }");

            var service = new AnalisisMejoraService(mockUow.Object, mockIA.Object);

            var resultado = await service.GenerarHojaRutaAsync(1);

            Assert.Equal(1, resultado.TotalComentariosAnalizados);
            Assert.NotNull(promptCapturado);
            Assert.Contains("Comentario válido", promptCapturado);
            Assert.Contains("Juez1", promptCapturado);
        }

        [Fact]
        public async Task PromptIncluyeNombreProyectoYComentarios()
        {
            var comentarios = new List<VotoExperto>
            {
                CreateVotoExperto(1, 1, "Buen trabajo en general", "Juez1")
            };

            var mockUow = CreateUnitOfWorkMock(comentarios);

            string? promptCapturado = null;
            var mockIA = new Mock<IIAProvider>();
            mockIA.Setup(p => p.AnalizarAsync(It.IsAny<string>()))
                  .Callback<string>(p => promptCapturado = p)
                  .ReturnsAsync("{ \"sugerencias\": [] }");

            var service = new AnalisisMejoraService(mockUow.Object, mockIA.Object);

            await service.GenerarHojaRutaAsync(1);

            Assert.NotNull(promptCapturado);
            Assert.Contains("Proyecto1", promptCapturado);
            Assert.Contains("Buen trabajo en general", promptCapturado);
            Assert.Contains("Juez1", promptCapturado);
        }

        [Fact]
        public async Task RespuestaIAInvalida_DevuelveListaVaciaSinExcepcion()
        {
            var comentarios = new List<VotoExperto>
            {
                CreateVotoExperto(1, 1, "Comentario válido", "Juez1")
            };

            var mockUow = CreateUnitOfWorkMock(comentarios);

            var mockIA = new Mock<IIAProvider>();
            mockIA.Setup(p => p.AnalizarAsync(It.IsAny<string>()))
                  .ReturnsAsync("esto no es JSON válido");

            var service = new AnalisisMejoraService(mockUow.Object, mockIA.Object);

            var resultado = await service.GenerarHojaRutaAsync(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.ProyectoId);
            Assert.Equal(1, resultado.TotalComentariosAnalizados);
            Assert.Empty(resultado.Sugerencias);
        }

        [Fact]
        public async Task ProyectoIdInvalido_LanzaArgumentException()
        {
            var mockUow = CreateUnitOfWorkMock(new List<VotoExperto>());
            var mockIA = new Mock<IIAProvider>();

            var service = new AnalisisMejoraService(mockUow.Object, mockIA.Object);

            await Assert.ThrowsAsync<ArgumentException>(() => service.GenerarHojaRutaAsync(0));
            await Assert.ThrowsAsync<ArgumentException>(() => service.GenerarHojaRutaAsync(-1));
        }

        [Fact]
        public async Task SugerenciasSeOrdenanPorPrioridad()
        {
            var comentarios = new List<VotoExperto>
            {
                CreateVotoExperto(1, 1, "Varios problemas", "Juez1")
            };

            var mockUow = CreateUnitOfWorkMock(comentarios);

            var mockIA = new Mock<IIAProvider>();
            mockIA.Setup(p => p.AnalizarAsync(It.IsAny<string>()))
                  .ReturnsAsync("""
                    {
                        "sugerencias": [
                            { "prioridad": 3, "categoria": "UX", "descripcion": "Mejorar interfaz", "accionRecomendada": "Rediseñar" },
                            { "prioridad": 1, "categoria": "Seguridad", "descripcion": "Fix crítica", "accionRecomendada": "Parchear" },
                            { "prioridad": 2, "categoria": "Performance", "descripcion": "Optimizar consultas", "accionRecomendada": "Añadir índices" }
                        ]
                    }
                    """);

            var service = new AnalisisMejoraService(mockUow.Object, mockIA.Object);

            var resultado = await service.GenerarHojaRutaAsync(1);

            Assert.Equal(3, resultado.Sugerencias.Count);
            Assert.Equal(1, resultado.Sugerencias[0].Prioridad);
            Assert.Equal("Seguridad", resultado.Sugerencias[0].Categoria);
            Assert.Equal(2, resultado.Sugerencias[1].Prioridad);
            Assert.Equal("Performance", resultado.Sugerencias[1].Categoria);
            Assert.Equal(3, resultado.Sugerencias[2].Prioridad);
            Assert.Equal("UX", resultado.Sugerencias[2].Categoria);
        }
    }
}
