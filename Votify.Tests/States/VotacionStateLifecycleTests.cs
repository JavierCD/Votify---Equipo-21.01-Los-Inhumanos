using Votify.Core.Models;
using Xunit;

namespace Votify.Tests.States
{
    public class VotacionStateLifecycleTests
    {
        [Fact]
        public void CicloCompleto_ProgramadaHastaResultadosPublicados()
        {
            var votacion = new Popular { Id = 1, Estado = "Programada" };
            votacion.FechaApertura = DateTime.UtcNow.AddDays(1);
            votacion.FechaCierre = DateTime.UtcNow.AddDays(2);

            Assert.Equal("Programada", votacion.Estado);
            Assert.IsType<ProgramadaState>(votacion.GetState());

            votacion.ForzarApertura();
            Assert.Equal("Abierta", votacion.Estado);
            Assert.False(votacion.EstaCerrada);
            Assert.IsType<AbiertaState>(votacion.GetState());

            votacion.PausarVotacion();
            Assert.Equal("Pausada", votacion.Estado);
            Assert.IsType<PausadaState>(votacion.GetState());

            votacion.ReanudarVotacion();
            Assert.Equal("Pausada", votacion.Estado);

            votacion.ForzarCierre();
            Assert.True(votacion.EstaCerrada);
            Assert.Equal("CerradaManual", votacion.Estado);
            Assert.IsType<CerradaManualState>(votacion.GetState());

            votacion.CompartirResultados();
            Assert.True(votacion.ResultadosPublicados);
            Assert.Equal("ResultadosPublicados", votacion.Estado);
            Assert.IsType<ResultadosPublicadosState>(votacion.GetState());
        }

        [Fact]
        public void CicloCompleto_CerradaNormalHastaResultadosPublicados()
        {
            var votacion = new Popular { Id = 1, Estado = "Programada" };
            votacion.FechaApertura = DateTime.UtcNow.AddDays(1);
            votacion.FechaCierre = DateTime.UtcNow.AddDays(2);

            votacion.ForzarApertura();
            Assert.Equal("Abierta", votacion.Estado);

            votacion.CerrarVotacion();
            Assert.True(votacion.EstaCerrada);
            Assert.Equal("Cerrada", votacion.Estado);
            Assert.IsType<CerradaState>(votacion.GetState());

            votacion.CompartirResultados();
            Assert.True(votacion.ResultadosPublicados);
            Assert.Equal("ResultadosPublicados", votacion.Estado);
            Assert.IsType<ResultadosPublicadosState>(votacion.GetState());
        }

        [Fact]
        public void Cerrada_Abrir_ReabreVotacion()
        {
            var votacion = new Popular { Id = 1, Estado = "Programada" };
            votacion.FechaApertura = DateTime.UtcNow.AddDays(1);
            votacion.FechaCierre = DateTime.UtcNow.AddDays(2);

            votacion.ForzarApertura();
            votacion.CerrarVotacion();
            Assert.True(votacion.EstaCerrada);
            Assert.Equal("Cerrada", votacion.Estado);

            votacion.ForzarApertura();
            Assert.False(votacion.EstaCerrada);
            Assert.Equal("Abierta", votacion.Estado);
            Assert.IsType<AbiertaState>(votacion.GetState());
        }

        [Fact]
        public void CerradaManual_Abrir_ReabreVotacion()
        {
            var votacion = new Popular { Id = 1, Estado = "Programada" };
            votacion.FechaApertura = DateTime.UtcNow.AddDays(1);
            votacion.FechaCierre = DateTime.UtcNow.AddDays(2);

            votacion.ForzarApertura();
            votacion.ForzarCierre();
            Assert.True(votacion.EstaCerrada);
            Assert.Equal("CerradaManual", votacion.Estado);

            votacion.ForzarApertura();
            Assert.False(votacion.EstaCerrada);
            Assert.Equal("Abierta", votacion.Estado);
            Assert.IsType<AbiertaState>(votacion.GetState());
        }

        [Fact]
        public void CerradaManual_Programar_VuelveAProgramada()
        {
            var votacion = new Popular { Id = 1, Estado = "Programada" };
            votacion.FechaApertura = DateTime.UtcNow.AddDays(1);
            votacion.FechaCierre = DateTime.UtcNow.AddDays(2);

            votacion.ForzarApertura();
            votacion.ForzarCierre();

            votacion.ForzarProgramada();
            Assert.False(votacion.EstaCerrada);
            Assert.Equal("Programada", votacion.Estado);
            Assert.IsType<ProgramadaState>(votacion.GetState());
        }

        [Fact]
        public void Pausada_CerrarManual_CierraDirectamente()
        {
            var votacion = new Popular { Id = 1, Estado = "Programada" };
            votacion.FechaApertura = DateTime.UtcNow.AddDays(1);
            votacion.FechaCierre = DateTime.UtcNow.AddDays(2);

            votacion.ForzarApertura();
            votacion.PausarVotacion();

            votacion.ForzarCierre();
            Assert.True(votacion.EstaCerrada);
            Assert.Equal("CerradaManual", votacion.Estado);
            Assert.IsType<CerradaManualState>(votacion.GetState());
        }

        [Fact]
        public void Pausada_Programar_VuelveAProgramada()
        {
            var votacion = new Popular { Id = 1, Estado = "Programada" };
            votacion.FechaApertura = DateTime.UtcNow.AddDays(1);
            votacion.FechaCierre = DateTime.UtcNow.AddDays(2);

            votacion.ForzarApertura();
            votacion.PausarVotacion();

            votacion.ForzarProgramada();
            Assert.False(votacion.EstaCerrada);
            Assert.Equal("Programada", votacion.Estado);
            Assert.IsType<ProgramadaState>(votacion.GetState());
        }
    }
}
