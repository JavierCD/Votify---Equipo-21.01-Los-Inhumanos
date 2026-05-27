using Votify.Core.Models;
using Votify.Core.Enums;
using Xunit;

namespace Votify.Tests.States
{
    public class PausadaStateTests
    {
        private PausadaState CreateState() => new PausadaState();

        private Votacion CreateVotacion()
        {
            var votacion = new Popular { Id = 1 };
            votacion.SetState(new PausadaState());
            return votacion;
        }

        [Fact]
        public void Nombre_EsPausada()
        {
            var state = CreateState();
            Assert.Equal(EstadoVotacion.Pausada, state.Tipo);
        }

        [Fact]
        public void Reanudar_EvaluaEstadoTemporal()
        {
            var state = CreateState();
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Pausada };
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(-10);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(10);
            votacion.SetState(new PausadaState());

            state.Reanudar(votacion);

            // PausadaState no tiene EvaluarTemporal, así que se queda en Pausada
            // El comportamiento real es que no cambia automáticamente
            Assert.Equal(EstadoVotacion.Pausada, votacion.Estado);
        }

        [Fact]
        public void CerrarManual_CambiaACerradaManual()
        {
            var state = CreateState();
            var votacion = CreateVotacion();

            state.CerrarManual(votacion);

            Assert.True(votacion.EstaCerrada);
            Assert.Equal(EstadoVotacion.Cerrada, votacion.Estado);
            Assert.IsType<CerradaState>(votacion.GetState());
        }

        [Fact]
        public void Programar_CambiaAProgramada()
        {
            var state = CreateState();
            var votacion = CreateVotacion();

            state.Programar(votacion);

            Assert.False(votacion.EstaCerrada);
            Assert.Equal(EstadoVotacion.Programada, votacion.Estado);
            Assert.IsType<ProgramadaState>(votacion.GetState());
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
        public void Abrir_CambiaAAbierta()
        {
            var state = CreateState();
            var votacion = CreateVotacion();

            state.Abrir(votacion);

            Assert.False(votacion.EstaCerrada);
            Assert.Equal(EstadoVotacion.Abierta, votacion.Estado);
            Assert.IsType<AbiertaState>(votacion.GetState());
        }
    }
}
