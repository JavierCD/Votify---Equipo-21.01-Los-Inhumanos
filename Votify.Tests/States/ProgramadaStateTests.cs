using Votify.Core.Models;
using Xunit;

namespace Votify.Tests.States
{
    public class ProgramadaStateTests
    {
        private ProgramadaState CreateState() => new ProgramadaState();

        private Votacion CreateVotacion()
        {
            var votacion = new Popular { Id = 1 };
            votacion.SetState(new ProgramadaState());
            return votacion;
        }

        [Fact]
        public void Nombre_EsProgramada()
        {
            var state = CreateState();
            Assert.Equal("Programada", state.Nombre);
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
        public void EvaluarTemporal_CuandoEnRango_CambiaAAbierta()
        {
            var state = CreateState();
            var votacion = CreateVotacion();
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(-10);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(10);

            state.EvaluarTemporal(votacion, DateTime.UtcNow);

            Assert.Equal("Abierta", votacion.Estado);
            Assert.IsType<AbiertaState>(votacion.GetState());
        }

        [Fact]
        public void EvaluarTemporal_CuandoFueraDeRango_CambiaACerrada()
        {
            var state = CreateState();
            var votacion = CreateVotacion();
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(-20);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(-10);

            state.EvaluarTemporal(votacion, DateTime.UtcNow);

            Assert.True(votacion.EstaCerrada);
            Assert.Equal("Cerrada", votacion.Estado);
            Assert.IsType<CerradaState>(votacion.GetState());
        }

        [Fact]
        public void EvaluarTemporal_CuandoAntesDeApertura_NoCambiaEstado()
        {
            var state = CreateState();
            var votacion = CreateVotacion();
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(10);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(20);

            state.EvaluarTemporal(votacion, DateTime.UtcNow);

            Assert.Equal("Programada", votacion.Estado);
            Assert.IsType<ProgramadaState>(votacion.GetState());
        }

        [Fact]
        public void PuedeVotar_CuandoEnRango_DelegaAAbiertaState()
        {
            var state = CreateState();
            var votacion = CreateVotacion();
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(-10);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(10);

            var resultado = state.PuedeVotar(DateTime.UtcNow, votacion);

            Assert.True(resultado);
        }

        [Fact]
        public void PuedeVotar_CuandoFueraDeRango_DelegaAAbiertaState()
        {
            var state = CreateState();
            var votacion = CreateVotacion();
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(10);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(20);

            var resultado = state.PuedeVotar(DateTime.UtcNow, votacion);

            Assert.False(resultado);
        }
    }
}
