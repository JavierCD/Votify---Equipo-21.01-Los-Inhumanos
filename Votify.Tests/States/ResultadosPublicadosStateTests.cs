using Votify.Core.Models;
using Xunit;

namespace Votify.Tests.States
{
    public class ResultadosPublicadosStateTests
    {
        private ResultadosPublicadosState CreateState() => new ResultadosPublicadosState();

        private Votacion CreateVotacion()
        {
            var votacion = new Popular { Id = 1 };
            votacion.SetState(new ResultadosPublicadosState());
            return votacion;
        }

        [Fact]
        public void Nombre_EsResultadosPublicados()
        {
            var state = CreateState();
            Assert.Equal("ResultadosPublicados", state.Nombre);
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
        public void Abrir_LanzaExcepcion()
        {
            var state = CreateState();
            var votacion = CreateVotacion();

            Assert.Throws<InvalidOperationException>(() => state.Abrir(votacion));
        }

        [Fact]
        public void Cerrar_LanzaExcepcion()
        {
            var state = CreateState();
            var votacion = CreateVotacion();

            Assert.Throws<InvalidOperationException>(() => state.Cerrar(votacion));
        }

        [Fact]
        public void CerrarManual_LanzaExcepcion()
        {
            var state = CreateState();
            var votacion = CreateVotacion();

            Assert.Throws<InvalidOperationException>(() => state.CerrarManual(votacion));
        }

        [Fact]
        public void Pausar_LanzaExcepcion()
        {
            var state = CreateState();
            var votacion = CreateVotacion();

            Assert.Throws<InvalidOperationException>(() => state.Pausar(votacion));
        }
    }
}
