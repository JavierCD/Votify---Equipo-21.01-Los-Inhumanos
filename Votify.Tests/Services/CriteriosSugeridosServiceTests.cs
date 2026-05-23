using Moq;
using Votify.Core.Interfaces;
using Votify.Services.Implementations.Analysis;
using Votify.Services.Interfaces;
using Votify.Services.Models.Requests;
using Xunit;

namespace Votify.Tests.Services
{
    public class CriteriosSugeridosServiceTests
    {
        [Fact]
        public async Task SugerirCriteriosAsync_CuandoIAFunciona_DevuelveCriterios()
        {
            var mockIA = new Mock<IIAProvider>();
            mockIA.Setup(p => p.AnalizarAsync(It.IsAny<string>()))
                  .ReturnsAsync(@"[{""nombre"": ""Innovación"", ""peso"": 30}, {""nombre"": ""Calidad"", ""peso"": 40}, {""nombre"": ""Presentación"", ""peso"": 30}]");

            var service = new CriteriosSugeridosService(mockIA.Object);

            var resultado = await service.SugerirCriteriosAsync("Hackathon de IA", "Descripción del hackathon");

            Assert.Equal(3, resultado.Count);
            Assert.Equal("Innovación", resultado[0].Nombre);
            Assert.Equal(30, resultado[0].Peso);
        }

        [Fact]
        public async Task SugerirCriteriosAsync_CuandoIAFalla_LanzaExcepcion()
        {
            var mockIA = new Mock<IIAProvider>();
            mockIA.Setup(p => p.AnalizarAsync(It.IsAny<string>()))
                  .ThrowsAsync(new InvalidOperationException("Error de IA"));

            var service = new CriteriosSugeridosService(mockIA.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.SugerirCriteriosAsync("Hackathon", "Descripción"));
        }

        [Fact]
        public async Task SugerirCriteriosAsync_CuandoRespuestaInvalida_DevuelveListaVacia()
        {
            var mockIA = new Mock<IIAProvider>();
            mockIA.Setup(p => p.AnalizarAsync(It.IsAny<string>()))
                  .ReturnsAsync("esto no es JSON válido");

            var service = new CriteriosSugeridosService(mockIA.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.SugerirCriteriosAsync("Hackathon", "Descripción"));
        }

        [Fact]
        public async Task SugerirCriteriosAsync_PromptIncluyeContexto()
        {
            var mockIA = new Mock<IIAProvider>();
            string? promptCapturado = null;
            mockIA.Setup(p => p.AnalizarAsync(It.IsAny<string>()))
                  .Callback<string>(p => promptCapturado = p)
                  .ReturnsAsync(@"[{""nombre"": ""Test"", ""peso"": 100}]");

            var service = new CriteriosSugeridosService(mockIA.Object);

            await service.SugerirCriteriosAsync("Hackathon de IA", "Criterio existente");

            Assert.NotNull(promptCapturado);
            Assert.Contains("Hackathon de IA", promptCapturado);
            Assert.Contains("Criterio existente", promptCapturado);
        }
    }
}
