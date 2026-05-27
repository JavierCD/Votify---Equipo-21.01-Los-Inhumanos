using Votify.Core.Models;
using Votify.Core.Enums;
using Xunit;

namespace Votify.Tests.States
{
    public class VotacionSincronizarEstadoTests
    {
        [Fact]
        public void SincronizarEstado_Programada_CreaProgramadaState()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Programada };
            var state = votacion.GetState();
            Assert.IsType<ProgramadaState>(state);
            Assert.Equal(EstadoVotacion.Programada, state.Tipo);
        }

        [Fact]
        public void SincronizarEstado_Abierta_CreaAbiertaState()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Abierta };
            var state = votacion.GetState();
            Assert.IsType<AbiertaState>(state);
            Assert.Equal(EstadoVotacion.Abierta, state.Tipo);
        }

        [Fact]
        public void SincronizarEstado_Cerrada_CreaCerradaState()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Cerrada };
            var state = votacion.GetState();
            Assert.IsType<CerradaState>(state);
            Assert.Equal(EstadoVotacion.Cerrada, state.Tipo);
        }

        [Fact]
        public void SincronizarEstado_CerradaManual_CreaCerradaManualState()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Cerrada };
            var state = votacion.GetState();
            Assert.IsType<CerradaState>(state);
            Assert.Equal(EstadoVotacion.Cerrada, state.Tipo);
        }

        [Fact]
        public void SincronizarEstado_Pausada_CreaPausadaState()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Pausada };
            var state = votacion.GetState();
            Assert.IsType<PausadaState>(state);
            Assert.Equal(EstadoVotacion.Pausada, state.Tipo);
        }

        [Fact]
        public void SincronizarEstado_ResultadosPublicados_CreaResultadosPublicadosState()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.ResultadosPublicados };
            var state = votacion.GetState();
            Assert.IsType<ResultadosPublicadosState>(state);
            Assert.Equal(EstadoVotacion.ResultadosPublicados, state.Tipo);
        }

        [Fact]
        public void SincronizarEstado_ValorDesconocido_CreaProgramadaStatePorDefecto()
        {
            var votacion = new Popular { Id = 1, Estado = (EstadoVotacion)999 };
            var state = votacion.GetState();
            Assert.IsType<ProgramadaState>(state);
        }

        [Fact]
        public void SincronizarEstado_ValorNull_CreaProgramadaStatePorDefecto()
        {
            var votacion = new Popular { Id = 1 };
            votacion.Estado = default(EstadoVotacion);
            var state = votacion.GetState();
            Assert.IsType<ProgramadaState>(state);
        }

        [Fact]
        public void SincronizarEstado_ValorVacio_CreaProgramadaStatePorDefecto()
        {
            var votacion = new Popular { Id = 1, Estado = default };
            var state = votacion.GetState();
            Assert.IsType<ProgramadaState>(state);
        }

        [Fact]
        public void SetState_SincronizaStringEstadoConNombreDelState()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Programada };
            votacion.SetState(new AbiertaState());
            Assert.Equal(EstadoVotacion.Abierta, votacion.Estado);

            votacion.SetState(new CerradaState());
            Assert.Equal(EstadoVotacion.Cerrada, votacion.Estado);

            votacion.SetState(new PausadaState());
            Assert.Equal(EstadoVotacion.Pausada, votacion.Estado);
        }

        [Fact]
        public void GetState_MultipleLlamadas_DevuelveMismoState()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Abierta };
            var state1 = votacion.GetState();
            var state2 = votacion.GetState();
            Assert.Same(state1, state2);
        }

        [Fact]
        public void Transicion_CambiaEstadoYStateInternamente()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Programada };
            votacion.FechaApertura = DateTime.UtcNow.AddDays(1);
            votacion.FechaCierre = DateTime.UtcNow.AddDays(2);

            Assert.Equal(EstadoVotacion.Programada, votacion.Estado);
            Assert.IsType<ProgramadaState>(votacion.GetState());

            votacion.ForzarApertura();

            Assert.Equal(EstadoVotacion.Abierta, votacion.Estado);
            Assert.IsType<AbiertaState>(votacion.GetState());

            var stateRecuperado = votacion.GetState();
            Assert.Equal(EstadoVotacion.Abierta, stateRecuperado.Tipo);
        }
    }
}
