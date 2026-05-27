using Votify.Core.Models;
using Votify.Core.Enums;
using Xunit;

namespace Votify.Tests.States
{
    public class CerradaStateTests
    {
        private CerradaState CreateState() => new CerradaState();

        private Votacion CreateVotacion()
        {
            var votacion = new Popular { Id = 1 };
            votacion.SetState(new CerradaState());
            return votacion;
        }

        [Fact]
        public void Nombre_EsCerrada()
        {
            var state = CreateState();
            Assert.Equal(EstadoVotacion.Cerrada, state.Tipo);
        }

        [Fact]
        public void Abrir_CambiaAAbierta()
        {
            var state = CreateState();
            var votacion = CreateVotacion();
            votacion.FechaCierre = DateTime.UtcNow.AddDays(1);

            state.Abrir(votacion);

            Assert.False(votacion.EstaCerrada);
            Assert.Equal(EstadoVotacion.Abierta, votacion.Estado);
            Assert.IsType<AbiertaState>(votacion.GetState());
        }

        [Fact]
        public void CerrarManual_EsIdempotente()
        {
            var state = CreateState();
            var votacion = CreateVotacion();

            var ex = Record.Exception(() => state.CerrarManual(votacion));
            Assert.Null(ex);
            Assert.Equal(EstadoVotacion.Cerrada, votacion.Estado);
        }

        [Fact]
        public void PublicarResultados_CambiaAResultadosPublicados()
        {
            var state = CreateState();
            var votacion = CreateVotacion();
            votacion.ResultadosPublicados = false;

            state.PublicarResultados(votacion);

            Assert.True(votacion.ResultadosPublicados);
            Assert.Equal(EstadoVotacion.ResultadosPublicados, votacion.Estado);
            Assert.IsType<ResultadosPublicadosState>(votacion.GetState());
        }

        [Fact]
        public void Pausar_CambiaAPausada()
        {
            var state = CreateState();
            var votacion = CreateVotacion();

            state.Pausar(votacion);

            Assert.Equal(EstadoVotacion.Pausada, votacion.Estado);
            Assert.IsType<PausadaState>(votacion.GetState());
        }
    }
}
