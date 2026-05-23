using Votify.Core.Models;
using Xunit;

namespace Votify.Tests.States
{
    public class AbiertaStateTests
    {
        private AbiertaState CreateState() => new AbiertaState();

        private Votacion CreateVotacion()
        {
            var votacion = new Popular { Id = 1 };
            votacion.SetState(new AbiertaState());
            return votacion;
        }

        [Fact]
        public void Nombre_EsAbierta()
        {
            var state = CreateState();
            Assert.Equal("Abierta", state.Nombre);
        }

        [Fact]
        public void Cerrar_CambiaACerrada()
        {
            var state = CreateState();
            var votacion = CreateVotacion();
            votacion.EstaCerrada = false;

            state.Cerrar(votacion);

            Assert.True(votacion.EstaCerrada);
            Assert.Equal("Cerrada", votacion.Estado);
            Assert.IsType<CerradaState>(votacion.GetState());
        }

        [Fact]
        public void Cerrar_CuandoYaEstaCerrada_LanzaExcepcion()
        {
            var state = CreateState();
            var votacion = CreateVotacion();
            votacion.EstaCerrada = true;

            Assert.Throws<InvalidOperationException>(() => state.Cerrar(votacion));
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
        public void Pausar_CambiaAPausada()
        {
            var state = CreateState();
            var votacion = CreateVotacion();

            state.Pausar(votacion);

            Assert.Equal("Pausada", votacion.Estado);
            Assert.IsType<PausadaState>(votacion.GetState());
        }

        [Fact]
        public void EvaluarTemporal_CuandoFechaCierrePaso_CambiaACerrada()
        {
            var state = CreateState();
            var votacion = CreateVotacion();
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(-1);

            state.EvaluarTemporal(votacion, DateTime.UtcNow);

            Assert.True(votacion.EstaCerrada);
            Assert.Equal("Cerrada", votacion.Estado);
            Assert.IsType<CerradaState>(votacion.GetState());
        }

        [Fact]
        public void EvaluarTemporal_CuandoFechaCierreNoPaso_NoCambiaEstado()
        {
            var state = CreateState();
            var votacion = CreateVotacion();
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(10);

            state.EvaluarTemporal(votacion, DateTime.UtcNow);

            Assert.Equal("Abierta", votacion.Estado);
            Assert.IsType<AbiertaState>(votacion.GetState());
        }

        [Fact]
        public void PuedeVotar_CuandoEstaEnRango_DevuelveTrue()
        {
            var state = CreateState();
            var votacion = CreateVotacion();
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(-10);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(10);

            var resultado = state.PuedeVotar(DateTime.UtcNow, votacion);

            Assert.True(resultado);
        }

        [Fact]
        public void PuedeVotar_CuandoFueraDeRango_DevuelveFalse()
        {
            var state = CreateState();
            var votacion = CreateVotacion();
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(10);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(20);

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
    }
}
