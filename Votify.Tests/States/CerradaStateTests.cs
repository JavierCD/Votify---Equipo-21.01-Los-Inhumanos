using Votify.Core.Models;
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
            Assert.Equal("Cerrada", state.Nombre);
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
        public void CerrarManual_CambiaACerradaManual()
        {
            var state = CreateState();
            var votacion = CreateVotacion();

            state.CerrarManual(votacion);

            Assert.True(votacion.EstaCerrada);
            Assert.Equal("CerradaManual", votacion.Estado);
            Assert.IsType<CerradaManualState>(votacion.GetState());
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
