using Votify.Core.Models;
using Xunit;

namespace Votify.Tests.States
{
    public class CerradaManualStateTests
    {
        private CerradaManualState CreateState() => new CerradaManualState();

        private Votacion CreateVotacion()
        {
            var votacion = new Popular { Id = 1 };
            votacion.SetState(new CerradaManualState());
            return votacion;
        }

        [Fact]
        public void Nombre_EsCerradaManual()
        {
            var state = CreateState();
            Assert.Equal("CerradaManual", state.Nombre);
        }

        [Fact]
        public void Abrir_CambiaAAbierta()
        {
            var state = CreateState();
            var votacion = CreateVotacion();
            votacion.FechaCierre = DateTime.UtcNow.AddDays(1);

            state.Abrir(votacion);

            Assert.False(votacion.EstaCerrada);
            Assert.Equal("Abierta", votacion.Estado);
            Assert.IsType<AbiertaState>(votacion.GetState());
        }

        [Fact]
        public void Programar_CambiaAProgramada()
        {
            var state = CreateState();
            var votacion = CreateVotacion();

            state.Programar(votacion);

            Assert.False(votacion.EstaCerrada);
            Assert.Equal("Programada", votacion.Estado);
            Assert.IsType<ProgramadaState>(votacion.GetState());
        }

        [Fact]
        public void PublicarResultados_CambiaAResultadosPublicados()
        {
            var state = CreateState();
            var votacion = CreateVotacion();
            votacion.ResultadosPublicados = false;

            state.PublicarResultados(votacion);

            Assert.True(votacion.ResultadosPublicados);
            Assert.Equal("ResultadosPublicados", votacion.Estado);
            Assert.IsType<ResultadosPublicadosState>(votacion.GetState());
        }

        [Fact]
        public void Cerrar_CambiaACerrada()
        {
            var state = CreateState();
            var votacion = CreateVotacion();

            state.Cerrar(votacion);

            Assert.True(votacion.EstaCerrada);
            Assert.Equal("Cerrada", votacion.Estado);
            Assert.IsType<CerradaState>(votacion.GetState());
        }

        [Fact]
        public void Pausar_CambiaAPausada()
        {
            var state = CreateState();
            var votacion = CreateVotacion();

            state.Pausar(votacion);

            Assert.Equal("Pausada", votacion.Estado);
            Assert.IsType<PausadaState>(votacion.GetState());
        }
    }
}
