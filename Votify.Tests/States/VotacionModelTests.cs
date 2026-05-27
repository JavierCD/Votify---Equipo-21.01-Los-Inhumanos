using Votify.Core.Models;
using Votify.Core.Enums;
using Xunit;

namespace Votify.Tests.States
{
    public class VotacionModelTests
    {
        [Fact]
        public void ConfigurarFechas_FechasValidas_ConfiguraYEvalua()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Programada };
            var apertura = DateTime.UtcNow.AddMinutes(-10);
            var cierre = DateTime.UtcNow.AddMinutes(10);

            votacion.ConfigurarFechas(apertura, cierre);

            Assert.Equal(apertura.ToUniversalTime(), votacion.FechaApertura);
            Assert.Equal(cierre.ToUniversalTime(), votacion.FechaCierre);
            Assert.Equal(EstadoVotacion.Abierta, votacion.Estado);
        }

        [Fact]
        public void ConfigurarFechas_AperturaMayorQueCierre_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Programada };
            var apertura = DateTime.UtcNow.AddDays(2);
            var cierre = DateTime.UtcNow.AddDays(1);

            Assert.Throws<ArgumentException>(() => votacion.ConfigurarFechas(apertura, cierre));
        }

        [Fact]
        public void ConfigurarFechas_FechasIguales_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Programada };
            var fecha = DateTime.UtcNow.AddDays(1);

            Assert.Throws<ArgumentException>(() => votacion.ConfigurarFechas(fecha, fecha));
        }

        [Fact]
        public void EvaluarEstadoTemporal_FueraDeRango_CambiaEstado()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Programada };
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(-20);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(-10);

            votacion.EvaluarEstadoTemporal(DateTime.UtcNow);

            Assert.True(votacion.EstaCerrada);
            Assert.Equal(EstadoVotacion.Cerrada, votacion.Estado);
        }

        [Fact]
        public void EvaluarEstadoTemporal_DentroDeRango_CambiaAAbierta()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Programada };
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(-10);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(10);

            votacion.EvaluarEstadoTemporal(DateTime.UtcNow);

            Assert.Equal(EstadoVotacion.Abierta, votacion.Estado);
        }

        [Fact]
        public void PuedeVotar_DentroDeRango_DevuelveTrue()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Abierta };
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(-10);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(10);

            var resultado = votacion.PuedeVotar(DateTime.UtcNow);

            Assert.True(resultado);
        }

        [Fact]
        public void PuedeVotar_FueraDeRango_DevuelveFalse()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Abierta };
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(10);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(20);

            var resultado = votacion.PuedeVotar(DateTime.UtcNow);

            Assert.False(resultado);
        }

        [Fact]
        public void PuedeVotar_Cerrada_DevuelveFalse()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Cerrada };
            votacion.EstaCerrada = true;

            var resultado = votacion.PuedeVotar(DateTime.UtcNow);

            Assert.False(resultado);
        }

        [Fact]
        public void PuedeVotar_Pausada_DevuelveFalse()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Pausada };

            var resultado = votacion.PuedeVotar(DateTime.UtcNow);

            Assert.False(resultado);
        }

        [Fact]
        public void PuedeVotar_ResultadosPublicados_DevuelveFalse()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.ResultadosPublicados };

            var resultado = votacion.PuedeVotar(DateTime.UtcNow);

            Assert.False(resultado);
        }

        [Fact]
        public void PuedeVotar_Programada_FueraDeRango_DevuelveFalse()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Programada };
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(10);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(20);

            var resultado = votacion.PuedeVotar(DateTime.UtcNow);

            Assert.False(resultado);
        }

        [Fact]
        public void PuedeVotar_Programada_DentroDeRango_DevuelveTrue()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Programada };
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(-10);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(10);

            var resultado = votacion.PuedeVotar(DateTime.UtcNow);

            Assert.True(resultado);
        }

        [Fact]
        public void CerrarVotacion_CuandoYaEstaCerrada_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Abierta };
            votacion.EstaCerrada = true;

            Assert.Throws<InvalidOperationException>(() => votacion.CerrarVotacion());
        }

        [Fact]
        public void ForzarCierre_SiempreFuncionaDesdeAbierta()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Abierta };

            votacion.ForzarCierre();

            Assert.True(votacion.EstaCerrada);
            Assert.Equal(EstadoVotacion.Cerrada, votacion.Estado);
        }

        [Fact]
        public void ForzarCierre_SiempreFuncionaDesdeProgramada()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Programada };

            votacion.ForzarCierre();

            Assert.True(votacion.EstaCerrada);
            Assert.Equal(EstadoVotacion.Cerrada, votacion.Estado);
        }
    }
}
