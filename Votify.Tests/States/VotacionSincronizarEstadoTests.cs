using Votify.Core.Models;
using Xunit;

namespace Votify.Tests.States
{
    public class VotacionSincronizarEstadoTests
    {
        [Fact]
        public void SincronizarEstado_Programada_CreaProgramadaState()
        {
            var votacion = new Popular { Id = 1, Estado = "Programada" };
            var state = votacion.GetState();
            Assert.IsType<ProgramadaState>(state);
            Assert.Equal("Programada", state.Nombre);
        }

        [Fact]
        public void SincronizarEstado_Abierta_CreaAbiertaState()
        {
            var votacion = new Popular { Id = 1, Estado = "Abierta" };
            var state = votacion.GetState();
            Assert.IsType<AbiertaState>(state);
            Assert.Equal("Abierta", state.Nombre);
        }

        [Fact]
        public void SincronizarEstado_Cerrada_CreaCerradaState()
        {
            var votacion = new Popular { Id = 1, Estado = "Cerrada" };
            var state = votacion.GetState();
            Assert.IsType<CerradaState>(state);
            Assert.Equal("Cerrada", state.Nombre);
        }

        [Fact]
        public void SincronizarEstado_CerradaManual_CreaCerradaManualState()
        {
            var votacion = new Popular { Id = 1, Estado = "CerradaManual" };
            var state = votacion.GetState();
            Assert.IsType<CerradaManualState>(state);
            Assert.Equal("CerradaManual", state.Nombre);
        }

        [Fact]
        public void SincronizarEstado_Pausada_CreaPausadaState()
        {
            var votacion = new Popular { Id = 1, Estado = "Pausada" };
            var state = votacion.GetState();
            Assert.IsType<PausadaState>(state);
            Assert.Equal("Pausada", state.Nombre);
        }

        [Fact]
        public void SincronizarEstado_ResultadosPublicados_CreaResultadosPublicadosState()
        {
            var votacion = new Popular { Id = 1, Estado = "ResultadosPublicados" };
            var state = votacion.GetState();
            Assert.IsType<ResultadosPublicadosState>(state);
            Assert.Equal("ResultadosPublicados", state.Nombre);
        }

        [Fact]
        public void SincronizarEstado_ValorDesconocido_CreaProgramadaStatePorDefecto()
        {
            var votacion = new Popular { Id = 1, Estado = "ValorRaro" };
            var state = votacion.GetState();
            Assert.IsType<ProgramadaState>(state);
        }

        [Fact]
        public void SincronizarEstado_ValorNull_CreaProgramadaStatePorDefecto()
        {
            var votacion = new Popular { Id = 1 };
            votacion.Estado = null!;
            var state = votacion.GetState();
            Assert.IsType<ProgramadaState>(state);
        }

        [Fact]
        public void SincronizarEstado_ValorVacio_CreaProgramadaStatePorDefecto()
        {
            var votacion = new Popular { Id = 1, Estado = "" };
            var state = votacion.GetState();
            Assert.IsType<ProgramadaState>(state);
        }

        [Fact]
        public void SetState_SincronizaStringEstadoConNombreDelState()
        {
            var votacion = new Popular { Id = 1, Estado = "Programada" };
            votacion.SetState(new AbiertaState());
            Assert.Equal("Abierta", votacion.Estado);

            votacion.SetState(new CerradaState());
            Assert.Equal("Cerrada", votacion.Estado);

            votacion.SetState(new PausadaState());
            Assert.Equal("Pausada", votacion.Estado);
        }

        [Fact]
        public void GetState_MultipleLlamadas_DevuelveMismoState()
        {
            var votacion = new Popular { Id = 1, Estado = "Abierta" };
            var state1 = votacion.GetState();
            var state2 = votacion.GetState();
            Assert.Same(state1, state2);
        }

        [Fact]
        public void Transicion_CambiaEstadoYStateInternamente()
        {
            var votacion = new Popular { Id = 1, Estado = "Programada" };
            votacion.FechaApertura = DateTime.UtcNow.AddDays(1);
            votacion.FechaCierre = DateTime.UtcNow.AddDays(2);

            Assert.Equal("Programada", votacion.Estado);
            Assert.IsType<ProgramadaState>(votacion.GetState());

            votacion.ForzarApertura();

            Assert.Equal("Abierta", votacion.Estado);
            Assert.IsType<AbiertaState>(votacion.GetState());

            var stateRecuperado = votacion.GetState();
            Assert.Equal("Abierta", stateRecuperado.Nombre);
        }
    }
}
