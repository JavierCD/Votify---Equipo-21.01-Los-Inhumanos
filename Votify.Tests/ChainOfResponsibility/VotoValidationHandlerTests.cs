using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Core.Enums;
using Xunit;

namespace Votify.Tests.ChainOfResponsibility
{
    public class VotoValidationHandlerTests
    {
        #region VotacionExistsHandler Tests

        [Fact]
        public async Task VotacionExistsHandler_CuandoExiste_PasaSiguiente()
        {
            var votacion = new Puntuacion { Id = 1 };
            var mockObtener = new Func<int, Task<Puntuacion?>>(id => Task.FromResult((Puntuacion?)votacion));
            var handler = new VotacionExistsHandler<Puntuacion>(mockObtener);
            var nextMock = new Mock<IVotoValidationHandler>();
            handler.SetNext(nextMock.Object);

            var context = new VotoValidationContext { VotacionId = 1 };
            await handler.HandleAsync(context);

            Assert.Same(votacion, context.Votacion);
            nextMock.Verify(n => n.HandleAsync(context), Times.Once);
        }

        [Fact]
        public async Task VotacionExistsHandler_CuandoNoExiste_LanzaExcepcion()
        {
            var mockObtener = new Func<int, Task<Puntuacion?>>(id => Task.FromResult((Puntuacion?)null));
            var handler = new VotacionExistsHandler<Puntuacion>(mockObtener);

            var context = new VotoValidationContext { VotacionId = 999 };
            await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(context));
        }

        #endregion

        #region VotacionAbiertaHandler Tests

        [Fact]
        public async Task VotacionAbiertaHandler_CuandoPuedeVotar_PasaSiguiente()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Abierta };
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(-10);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(10);

            var handler = new VotacionAbiertaHandler();
            var nextMock = new Mock<IVotoValidationHandler>();
            handler.SetNext(nextMock.Object);

            var context = new VotoValidationContext { Votacion = votacion };
            await handler.HandleAsync(context);

            nextMock.Verify(n => n.HandleAsync(context), Times.Once);
        }

        [Fact]
        public async Task VotacionAbiertaHandler_CuandoNoPuedeVotar_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Abierta };
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(10);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(20);

            var handler = new VotacionAbiertaHandler();
            var context = new VotoValidationContext { Votacion = votacion };

            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(context));
        }

        [Fact]
        public async Task VotacionAbiertaHandler_CuandoVotacionNull_LanzaExcepcion()
        {
            var handler = new VotacionAbiertaHandler();
            var context = new VotoValidationContext { Votacion = null };

            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(context));
        }

        #endregion

        #region SingleVoteRestrictionHandler Tests

        [Fact]
        public async Task SingleVoteRestrictionHandler_CuandoEmailYaVoto_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, RestriccionVotoUnico = true };
            var mockEmailYaVoto = new Func<int, string, Task<bool>>((v, e) => Task.FromResult(true));
            var handler = new SingleVoteRestrictionHandler(mockEmailYaVoto);

            var context = new VotoValidationContext { Votacion = votacion, VotacionId = 1, Email = "test@test.com" };
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(context));
        }

        [Fact]
        public async Task SingleVoteRestrictionHandler_CuandoEmailNoVoto_PasaSiguiente()
        {
            var votacion = new Popular { Id = 1, RestriccionVotoUnico = true };
            var mockEmailYaVoto = new Func<int, string, Task<bool>>((v, e) => Task.FromResult(false));
            var handler = new SingleVoteRestrictionHandler(mockEmailYaVoto);
            var nextMock = new Mock<IVotoValidationHandler>();
            handler.SetNext(nextMock.Object);

            var context = new VotoValidationContext { Votacion = votacion, VotacionId = 1, Email = "test@test.com" };
            await handler.HandleAsync(context);

            nextMock.Verify(n => n.HandleAsync(context), Times.Once);
        }

        [Fact]
        public async Task SingleVoteRestrictionHandler_CuandoSinRestriccion_PasaSiguiente()
        {
            var votacion = new Popular { Id = 1, RestriccionVotoUnico = false };
            var mockEmailYaVoto = new Func<int, string, Task<bool>>((v, e) => Task.FromResult(true));
            var handler = new SingleVoteRestrictionHandler(mockEmailYaVoto);
            var nextMock = new Mock<IVotoValidationHandler>();
            handler.SetNext(nextMock.Object);

            var context = new VotoValidationContext { Votacion = votacion, VotacionId = 1, Email = "test@test.com" };
            await handler.HandleAsync(context);

            nextMock.Verify(n => n.HandleAsync(context), Times.Once);
        }

        [Fact]
        public async Task SingleVoteRestrictionHandler_CuandoSinEmail_PasaSiguiente()
        {
            var votacion = new Popular { Id = 1, RestriccionVotoUnico = true };
            var mockEmailYaVoto = new Func<int, string, Task<bool>>((v, e) => Task.FromResult(true));
            var handler = new SingleVoteRestrictionHandler(mockEmailYaVoto);
            var nextMock = new Mock<IVotoValidationHandler>();
            handler.SetNext(nextMock.Object);

            var context = new VotoValidationContext { Votacion = votacion, VotacionId = 1, Email = null };
            await handler.HandleAsync(context);

            nextMock.Verify(n => n.HandleAsync(context), Times.Once);
        }

        #endregion

        #region ProyectosValidosHandler Tests

        [Fact]
        public async Task ProyectosValidosHandler_CuandoTodosValidos_PasaSiguiente()
        {
            var proyectos = new List<Proyecto>
            {
                new AiProject("P1", 1) { Id = 1 },
                new AiProject("P2", 1) { Id = 2 }
            };
            var mockObtener = new Func<int, Task<List<Proyecto>>>(id => Task.FromResult(proyectos));
            var mockObtenerIds = new Func<VotoValidationContext, IEnumerable<int>>(ctx => new[] { 1, 2 });
            var handler = new ProyectosValidosHandler(mockObtener, mockObtenerIds);
            var nextMock = new Mock<IVotoValidationHandler>();
            handler.SetNext(nextMock.Object);

            var context = new VotoValidationContext { Votacion = new Popular { Id = 1, CategoriaId = 1 } };
            await handler.HandleAsync(context);

            Assert.Equal(proyectos, context.ProyectosValidos);
            nextMock.Verify(n => n.HandleAsync(context), Times.Once);
        }

        [Fact]
        public async Task ProyectosValidosHandler_CuandoInvalido_LanzaExcepcion()
        {
            var proyectos = new List<Proyecto> { new AiProject("P1", 1) { Id = 1 } };
            var mockObtener = new Func<int, Task<List<Proyecto>>>(id => Task.FromResult(proyectos));
            var mockObtenerIds = new Func<VotoValidationContext, IEnumerable<int>>(ctx => new[] { 1, 999 });
            var handler = new ProyectosValidosHandler(mockObtener, mockObtenerIds);

            var context = new VotoValidationContext { Votacion = new Popular { Id = 1, CategoriaId = 1 } };
            await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(context));
        }

        #endregion

        #region NonEmptySelectionHandler Tests

        [Fact]
        public async Task NonEmptySelectionHandler_CuandoHaySeleccion_PasaSiguiente()
        {
            var handler = new NonEmptySelectionHandler(ctx => 3, "Debes seleccionar al menos uno.");
            var nextMock = new Mock<IVotoValidationHandler>();
            handler.SetNext(nextMock.Object);

            var context = new VotoValidationContext();
            await handler.HandleAsync(context);

            nextMock.Verify(n => n.HandleAsync(context), Times.Once);
        }

        [Fact]
        public async Task NonEmptySelectionHandler_CuandoSinSeleccion_LanzaExcepcion()
        {
            var handler = new NonEmptySelectionHandler(ctx => 0, "Debes seleccionar al menos uno.");
            var context = new VotoValidationContext();

            await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(context));
        }

        #endregion

        #region PuntuacionRangeHandler Tests

        [Fact]
        public async Task PuntuacionRangeHandler_CuandoPuntuacionesValidas_PasaSiguiente()
        {
            var votacion = new Puntuacion { Id = 1, ValorMax = 10 };
            var handler = new PuntuacionRangeHandler();
            var nextMock = new Mock<IVotoValidationHandler>();
            handler.SetNext(nextMock.Object);

            var context = new VotoValidationContext
            {
                Votacion = votacion,
                PuntuacionesPorProyecto = new Dictionary<int, int> { { 1, 5 }, { 2, 3 } }
            };
            await handler.HandleAsync(context);

            nextMock.Verify(n => n.HandleAsync(context), Times.Once);
        }

        [Fact]
        public async Task PuntuacionRangeHandler_CuandoPuntuacionFueraDeRango_LanzaExcepcion()
        {
            var votacion = new Puntuacion { Id = 1, ValorMax = 10 };
            var handler = new PuntuacionRangeHandler();
            var context = new VotoValidationContext
            {
                Votacion = votacion,
                PuntuacionesPorProyecto = new Dictionary<int, int> { { 1, 15 } }
            };

            await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(context));
        }

        [Fact]
        public async Task PuntuacionRangeHandler_CuandoSumaExcedeMaximo_LanzaExcepcion()
        {
            var votacion = new Puntuacion { Id = 1, ValorMax = 10 };
            var handler = new PuntuacionRangeHandler();
            var context = new VotoValidationContext
            {
                Votacion = votacion,
                PuntuacionesPorProyecto = new Dictionary<int, int> { { 1, 6 }, { 2, 5 } }
            };

            await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(context));
        }

        [Fact]
        public async Task PuntuacionRangeHandler_CuandoSinPuntuaciones_LanzaExcepcion()
        {
            var votacion = new Puntuacion { Id = 1, ValorMax = 10 };
            var handler = new PuntuacionRangeHandler();
            var context = new VotoValidationContext
            {
                Votacion = votacion,
                PuntuacionesPorProyecto = new Dictionary<int, int>()
            };

            await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(context));
        }

        #endregion

        #region MaxSelectionHandler Tests

        [Fact]
        public async Task MaxSelectionHandler_CuandoDentroDeLimite_PasaSiguiente()
        {
            var votacion = new Popular { Id = 1, MaxSelection = 3 };
            var handler = new MaxSelectionHandler(ctx => 2);
            var nextMock = new Mock<IVotoValidationHandler>();
            handler.SetNext(nextMock.Object);

            var context = new VotoValidationContext { Votacion = votacion };
            await handler.HandleAsync(context);

            nextMock.Verify(n => n.HandleAsync(context), Times.Once);
        }

        [Fact]
        public async Task MaxSelectionHandler_CuandoExcedeLimite_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, MaxSelection = 3 };
            var handler = new MaxSelectionHandler(ctx => 5);
            var context = new VotoValidationContext { Votacion = votacion };

            await Assert.ThrowsAsync<ArgumentException>(() => handler.HandleAsync(context));
        }

        #endregion

        #region Chain Integration Tests

        [Fact]
        public async Task ChainCompleta_Puntuacion_Valida_PasaTodosLosHandlers()
        {
            var votacion = new Puntuacion { Id = 1, CategoriaId = 1, ValorMax = 10, RestriccionVotoUnico = false };
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(-10);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(10);

            var proyectos = new List<Proyecto> { new AiProject("P1", 1) { Id = 1 }, new AiProject("P2", 1) { Id = 2 } };

            var chain = VotoValidationChainBuilder.BuildPuntuacionChain(
                obtenerVotacion: id => Task.FromResult((Puntuacion?)votacion),
                obtenerProyectos: catId => Task.FromResult(proyectos),
                emailYaVoto: (v, e) => Task.FromResult(false)
            );

            var context = new VotoValidationContext
            {
                VotacionId = 1,
                PuntuacionesPorProyecto = new Dictionary<int, int> { { 1, 5 }, { 2, 3 } }
            };

            await chain.HandleAsync(context);

            Assert.Same(votacion, context.Votacion);
            Assert.Equal(proyectos, context.ProyectosValidos);
        }

        [Fact]
        public async Task ChainCompleta_Popular_Valida_PasaTodosLosHandlers()
        {
            var votacion = new Popular { Id = 1, CategoriaId = 1, MaxSelection = 3, RestriccionVotoUnico = false };
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(-10);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(10);

            var proyectos = new List<Proyecto> { new AiProject("P1", 1) { Id = 1 }, new AiProject("P2", 1) { Id = 2 } };

            var chain = VotoValidationChainBuilder.BuildPopularChain(
                obtenerVotacion: id => Task.FromResult((Popular?)votacion),
                obtenerProyectos: catId => Task.FromResult(proyectos),
                emailYaVoto: (v, e) => Task.FromResult(false)
            );

            var context = new VotoValidationContext
            {
                VotacionId = 1,
                ProyectosSeleccionadosIds = new List<int> { 1, 2 }
            };

            await chain.HandleAsync(context);

            Assert.Same(votacion, context.Votacion);
            Assert.Equal(proyectos, context.ProyectosValidos);
        }

        [Fact]
        public async Task ChainCompleta_Multicriterio_Valida_PasaTodosLosHandlers()
        {
            var votacion = new Multicriterio { Id = 1, RestriccionVotoUnico = false };
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(-10);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(10);

            var chain = VotoValidationChainBuilder.BuildMulticriterioChain(
                obtenerVotacion: id => Task.FromResult((Multicriterio?)votacion),
                emailYaVoto: (v, e) => Task.FromResult(false)
            );

            var context = new VotoValidationContext { VotacionId = 1 };
            await chain.HandleAsync(context);

            Assert.Same(votacion, context.Votacion);
        }

        [Fact]
        public async Task ChainCompleta_Puntuacion_FallaEnVotacionNoExiste()
        {
            var chain = VotoValidationChainBuilder.BuildPuntuacionChain(
                obtenerVotacion: id => Task.FromResult((Puntuacion?)null),
                obtenerProyectos: catId => Task.FromResult(new List<Proyecto>()),
                emailYaVoto: (v, e) => Task.FromResult(false)
            );

            var context = new VotoValidationContext { VotacionId = 999 };
            await Assert.ThrowsAsync<ArgumentException>(() => chain.HandleAsync(context));
        }

        [Fact]
        public async Task ChainCompleta_Popular_FallaEnVotacionCerrada()
        {
            var votacion = new Popular { Id = 1, MaxSelection = 3, RestriccionVotoUnico = false };
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(10);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(20);

            var chain = VotoValidationChainBuilder.BuildPopularChain(
                obtenerVotacion: id => Task.FromResult((Popular?)votacion),
                obtenerProyectos: catId => Task.FromResult(new List<Proyecto>()),
                emailYaVoto: (v, e) => Task.FromResult(false)
            );

            var context = new VotoValidationContext { VotacionId = 1, ProyectosSeleccionadosIds = new List<int> { 1 } };
            await Assert.ThrowsAsync<InvalidOperationException>(() => chain.HandleAsync(context));
        }

        #endregion
    }
}
