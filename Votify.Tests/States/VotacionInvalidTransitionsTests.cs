using Votify.Core.Models;
using Votify.Core.Enums;
using Xunit;

namespace Votify.Tests.States
{
    public class VotacionInvalidTransitionsTests
    {
        [Fact]
        public void Abierta_Abrir_EsIdempotente()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Abierta };
            var ex = Record.Exception(() => votacion.ForzarApertura());
            Assert.Null(ex);
            Assert.Equal(EstadoVotacion.Abierta, votacion.Estado);
        }

        [Fact]
        public void Cerrada_Cerrar_EsIdempotente()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Cerrada };
            votacion.EstaCerrada = true;
            var ex = Record.Exception(() => votacion.CerrarVotacion());
            Assert.Null(ex);
        }

        [Fact]
        public void Cerrada_Pausar_CambiaAPausada()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Cerrada };
            votacion.PausarVotacion();
            Assert.Equal(EstadoVotacion.Pausada, votacion.Estado);
        }

        [Fact]
        public void CerradaManual_Cerrar_CambiaACerrada()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Cerrada };
            votacion.CerrarVotacion();
            Assert.Equal(EstadoVotacion.Cerrada, votacion.Estado);
        }

        [Fact]
        public void CerradaManual_Pausar_CambiaAPausada()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Cerrada };
            votacion.PausarVotacion();
            Assert.Equal(EstadoVotacion.Pausada, votacion.Estado);
        }

        [Fact]
        public void Pausada_Abrir_CambiaAAbierta()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Pausada };
            votacion.ForzarApertura();
            Assert.Equal(EstadoVotacion.Abierta, votacion.Estado);
        }

        [Fact]
        public void Pausada_Cerrar_CambiaACerrada()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Pausada };
            votacion.CerrarVotacion();
            Assert.True(votacion.EstaCerrada);
            Assert.Equal(EstadoVotacion.Cerrada, votacion.Estado);
        }

        [Fact]
        public void ResultadosPublicados_Abrir_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.ResultadosPublicados };
            Assert.Throws<InvalidOperationException>(() => votacion.ForzarApertura());
        }

        [Fact]
        public void ResultadosPublicados_Cerrar_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.ResultadosPublicados };
            Assert.Throws<InvalidOperationException>(() => votacion.CerrarVotacion());
        }

        [Fact]
        public void ResultadosPublicados_CerrarManual_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.ResultadosPublicados };
            Assert.Throws<InvalidOperationException>(() => votacion.ForzarCierre());
        }

        [Fact]
        public void ResultadosPublicados_Pausar_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.ResultadosPublicados };
            Assert.Throws<InvalidOperationException>(() => votacion.PausarVotacion());
        }

        [Fact]
        public void ResultadosPublicados_Programar_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.ResultadosPublicados };
            Assert.Throws<InvalidOperationException>(() => votacion.ForzarProgramada());
        }

        [Fact]
        public void ResultadosPublicados_PublicarResultados_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.ResultadosPublicados };
            Assert.Throws<InvalidOperationException>(() => votacion.CompartirResultados());
        }

        [Fact]
        public void Programada_CerrarVotacion_CierraDirectamente()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Programada };
            votacion.FechaApertura = DateTime.UtcNow.AddDays(1);
            votacion.FechaCierre = DateTime.UtcNow.AddDays(2);

            votacion.CerrarVotacion();
            Assert.True(votacion.EstaCerrada);
            Assert.Equal(EstadoVotacion.Cerrada, votacion.Estado);
        }

        [Fact]
        public void Abierta_PublicarResultados_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Abierta };
            Assert.Throws<InvalidOperationException>(() => votacion.CompartirResultados());
        }

        [Fact]
        public void Programada_PublicarResultados_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Programada };
            Assert.Throws<InvalidOperationException>(() => votacion.CompartirResultados());
        }

        [Fact]
        public void Pausada_PublicarResultados_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Pausada };
            Assert.Throws<InvalidOperationException>(() => votacion.CompartirResultados());
        }

        [Fact]
        public void Abierta_Programar_CambiaAProgramada()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Abierta };
            votacion.ForzarProgramada();
            Assert.Equal(EstadoVotacion.Programada, votacion.Estado);
        }

        [Fact]
        public void Cerrada_Programar_CambiaAProgramada()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Cerrada };
            votacion.ForzarProgramada();
            Assert.Equal(EstadoVotacion.Programada, votacion.Estado);
        }

        [Fact]
        public void Cerrada_Reanudar_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Cerrada };
            Assert.Throws<InvalidOperationException>(() => votacion.ReanudarVotacion());
        }

        [Fact]
        public void CerradaManual_Reanudar_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Cerrada };
            Assert.Throws<InvalidOperationException>(() => votacion.ReanudarVotacion());
        }

        [Fact]
        public void ResultadosPublicados_Reanudar_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.ResultadosPublicados };
            Assert.Throws<InvalidOperationException>(() => votacion.ReanudarVotacion());
        }

        [Fact]
        public void Programada_Reanudar_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Programada };
            Assert.Throws<InvalidOperationException>(() => votacion.ReanudarVotacion());
        }

        [Fact]
        public void Abierta_Reanudar_LanzaExcepcion()
        {
            var votacion = new Popular { Id = 1, Estado = EstadoVotacion.Abierta };
            Assert.Throws<InvalidOperationException>(() => votacion.ReanudarVotacion());
        }
    }
}
