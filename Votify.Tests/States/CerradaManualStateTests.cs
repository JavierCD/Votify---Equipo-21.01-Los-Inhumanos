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
        public void PuedeVotar_SiempreDevuelveFalse()
        {
            var state = CreateState();
            var votacion = CreateVotacion();

            var resultado = state.PuedeVotar(DateTime.UtcNow, votacion);

            Assert.False(resultado);
        }

        [Fact]
        public void Cerrar_LanzaExcepcion()
        {
            var state = CreateState();
            var votacion = CreateVotacion();

            Assert.Throws<InvalidOperationException>(() => state.Cerrar(votacion));
        }
    }
}
