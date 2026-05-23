using Votify.Core.Models;
using Xunit;

namespace Votify.Tests.States
{
    public class VotacionInvalidTransitionsTests
    {
        [Fact]
        public void Abierta_Abrir_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "Abierta" };
            Assert.Throws<InvalidOperationException>(() => votacion.ForzarApertura());
        }

        [Fact]
        public void Cerrada_Cerrar_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "Cerrada" };
            votacion.EstaCerrada = true;
            Assert.Throws<InvalidOperationException>(() => votacion.CerrarVotacion());
        }

        [Fact]
        public void Cerrada_Pausar_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "Cerrada" };
            Assert.Throws<InvalidOperationException>(() => votacion.PausarVotacion());
        }

        [Fact]
        public void CerradaManual_Cerrar_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "CerradaManual" };
            Assert.Throws<InvalidOperationException>(() => votacion.CerrarVotacion());
        }

        [Fact]
        public void CerradaManual_Pausar_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "CerradaManual" };
            Assert.Throws<InvalidOperationException>(() => votacion.PausarVotacion());
        }

        [Fact]
        public void Pausada_Abrir_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "Pausada" };
            Assert.Throws<InvalidOperationException>(() => votacion.ForzarApertura());
        }

        [Fact]
        public void Pausada_Cerrar_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "Pausada" };
            Assert.Throws<InvalidOperationException>(() => votacion.CerrarVotacion());
        }

        [Fact]
        public void ResultadosPublicados_Abrir_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "ResultadosPublicados" };
            Assert.Throws<InvalidOperationException>(() => votacion.ForzarApertura());
        }

        [Fact]
        public void ResultadosPublicados_Cerrar_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "ResultadosPublicados" };
            Assert.Throws<InvalidOperationException>(() => votacion.CerrarVotacion());
        }

        [Fact]
        public void ResultadosPublicados_CerrarManual_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "ResultadosPublicados" };
            Assert.Throws<InvalidOperationException>(() => votacion.ForzarCierre());
        }

        [Fact]
        public void ResultadosPublicados_Pausar_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "ResultadosPublicados" };
            Assert.Throws<InvalidOperationException>(() => votacion.PausarVotacion());
        }

        [Fact]
        public void ResultadosPublicados_Programar_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "ResultadosPublicados" };
            Assert.Throws<InvalidOperationException>(() => votacion.ForzarProgramada());
        }

        [Fact]
        public void ResultadosPublicados_PublicarResultados_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "ResultadosPublicados" };
            Assert.Throws<InvalidOperationException>(() => votacion.CompartirResultados());
        }

        [Fact]
        public void Programada_CerrarVotacion_CierraDirectamente()
        {
            var votacion = new Popular { Id = 1, Estado = "Programada" };
            votacion.FechaApertura = DateTime.UtcNow.AddDays(1);
            votacion.FechaCierre = DateTime.UtcNow.AddDays(2);

            votacion.CerrarVotacion();
            Assert.True(votacion.EstaCerrada);
            Assert.Equal("Cerrada", votacion.Estado);
        }

        [Fact]
        public void Abierta_PublicarResultados_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "Abierta" };
            Assert.Throws<InvalidOperationException>(() => votacion.CompartirResultados());
        }

        [Fact]
        public void Programada_PublicarResultados_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "Programada" };
            Assert.Throws<InvalidOperationException>(() => votacion.CompartirResultados());
        }

        [Fact]
        public void Pausada_PublicarResultados_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "Pausada" };
            Assert.Throws<InvalidOperationException>(() => votacion.CompartirResultados());
        }

        [Fact]
        public void Abierta_Programar_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "Abierta" };
            Assert.Throws<InvalidOperationException>(() => votacion.ForzarProgramada());
        }

        [Fact]
        public void Cerrada_Programar_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "Cerrada" };
            Assert.Throws<InvalidOperationException>(() => votacion.ForzarProgramada());
        }

        [Fact]
        public void Cerrada_Reanudar_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "Cerrada" };
            Assert.Throws<InvalidOperationException>(() => votacion.ReanudarVotacion());
        }

        [Fact]
        public void CerradaManual_Reanudar_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "CerradaManual" };
            Assert.Throws<InvalidOperationException>(() => votacion.ReanudarVotacion());
        }

        [Fact]
        public void ResultadosPublicados_Reanudar_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "ResultadosPublicados" };
            Assert.Throws<InvalidOperationException>(() => votacion.ReanudarVotacion());
        }

        [Fact]
        public void Programada_Reanudar_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "Programada" };
            Assert.Throws<InvalidOperationException>(() => votacion.ReanudarVotacion());
        }

        [Fact]
        public void Abierta_Reanudar_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = "Abierta" };
            Assert.Throws<InvalidOperationException>(() => votacion.ReanudarVotacion());
        }
    }
}
