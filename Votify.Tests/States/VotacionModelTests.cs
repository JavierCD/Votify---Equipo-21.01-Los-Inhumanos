using Votify.Core.Models;
using Xunit;

namespace Votify.Tests.States
{
    public class VotacionModelTests
    {
        [Fact]
        public void ConfigurarFechas_FechasValidas_ConfiguraYEvalua()
        {
            var votacion = new Popular { Id = 1, Estado = "Programada" };
            var apertura = DateTime.UtcNow.AddMinutes(-10);
            var cierre = DateTime.UtcNow.AddMinutes(10);

            votacion.ConfigurarFechas(apertura, cierre);

            Assert.Equal(apertura.ToUniversalTime(), votacion.FechaApertura);
            Assert.Equal(cierre.ToUniversalTime(), votacion.FechaCierre);
            Assert.Equal("Abierta", votacion.Estado);
        }

        [Fact]
        public void ConfigurarFechas_AperturaMayorQueCierre_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "Programada" };
            var apertura = DateTime.UtcNow.AddDays(2);
            var cierre = DateTime.UtcNow.AddDays(1);

            Assert.Throws<ArgumentException>(() => votacion.ConfigurarFechas(apertura, cierre));
        }

        [Fact]
        public void ConfigurarFechas_FechasIguales_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "Programada" };
            var fecha = DateTime.UtcNow.AddDays(1);

            Assert.Throws<ArgumentException>(() => votacion.ConfigurarFechas(fecha, fecha));
        }

        [Fact]
        public void EvaluarEstadoTemporal_FueraDeRango_CambiaEstado()
        {
            var votacion = new Popular { Id = 1, Estado = "Programada" };
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(-20);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(-10);

            votacion.EvaluarEstadoTemporal(DateTime.UtcNow);

            Assert.True(votacion.EstaCerrada);
            Assert.Equal("Cerrada", votacion.Estado);
        }

        [Fact]
        public void EvaluarEstadoTemporal_DentroDeRango_CambiaAAbierta()
        {
            var votacion = new Popular { Id = 1, Estado = "Programada" };
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(-10);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(10);

            votacion.EvaluarEstadoTemporal(DateTime.UtcNow);

            Assert.Equal("Abierta", votacion.Estado);
        }

        [Fact]
        public void PuedeVotar_DentroDeRango_DevuelveTrue()
        {
            var votacion = new Popular { Id = 1, Estado = "Abierta" };
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(-10);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(10);

            var resultado = votacion.PuedeVotar(DateTime.UtcNow);

            Assert.True(resultado);
        }

        [Fact]
        public void PuedeVotar_FueraDeRango_DevuelveFalse()
        {
            var votacion = new Popular { Id = 1, Estado = "Abierta" };
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(10);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(20);

            var resultado = votacion.PuedeVotar(DateTime.UtcNow);

            Assert.False(resultado);
        }

        [Fact]
        public void PuedeVotar_Cerrada_DevuelveFalse()
        {
            var votacion = new Popular { Id = 1, Estado = "Cerrada" };
            votacion.EstaCerrada = true;

            var resultado = votacion.PuedeVotar(DateTime.UtcNow);

            Assert.False(resultado);
        }

        [Fact]
        public void PuedeVotar_Pausada_DevuelveFalse()
        {
            var votacion = new Popular { Id = 1, Estado = "Pausada" };

            var resultado = votacion.PuedeVotar(DateTime.UtcNow);

            Assert.False(resultado);
        }

        [Fact]
        public void PuedeVotar_ResultadosPublicados_DevuelveFalse()
        {
            var votacion = new Popular { Id = 1, Estado = "ResultadosPublicados" };

            var resultado = votacion.PuedeVotar(DateTime.UtcNow);

            Assert.False(resultado);
        }

        [Fact]
        public void PuedeVotar_Programada_FueraDeRango_DevuelveFalse()
        {
            var votacion = new Popular { Id = 1, Estado = "Programada" };
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(10);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(20);

            var resultado = votacion.PuedeVotar(DateTime.UtcNow);

            Assert.False(resultado);
        }

        [Fact]
        public void PuedeVotar_Programada_DentroDeRango_DevuelveTrue()
        {
            var votacion = new Popular { Id = 1, Estado = "Programada" };
            votacion.FechaApertura = DateTime.UtcNow.AddMinutes(-10);
            votacion.FechaCierre = DateTime.UtcNow.AddMinutes(10);

            var resultado = votacion.PuedeVotar(DateTime.UtcNow);

            Assert.True(resultado);
        }

        [Fact]
        public void CerrarVotacion_CuandoYaEstaCerrada_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "Abierta" };
            votacion.EstaCerrada = true;

            Assert.Throws<InvalidOperationException>(() => votacion.CerrarVotacion());
        }

        [Fact]
        public void ForzarCierre_SiempreFuncionaDesdeAbierta()
        {
            var votacion = new Popular { Id = 1, Estado = "Abierta" };

            votacion.ForzarCierre();

            Assert.True(votacion.EstaCerrada);
            Assert.Equal("CerradaManual", votacion.Estado);
        }

        [Fact]
        public void ForzarCierre_SiempreFuncionaDesdeProgramada()
        {
            var votacion = new Popular { Id = 1, Estado = "Programada" };

            votacion.ForzarCierre();

            Assert.True(votacion.EstaCerrada);
            Assert.Equal("CerradaManual", votacion.Estado);
        }
    }
}
