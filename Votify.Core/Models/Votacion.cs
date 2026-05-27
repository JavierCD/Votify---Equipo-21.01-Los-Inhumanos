using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Enums;

namespace Votify.Core.Models
{
    public abstract class Votacion
    {
        public int Id { get; set; }
        public int EventoId { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime FechaCierre { get; set; }
        public EstadoVotacion Estado { get; set; } = EstadoVotacion.Programada;
        public List<Voto> Votos { get; set; } = new List<Voto>();
        public Categoria Categoria { get; set; }
        public virtual ICollection<Juez> JuecesAutorizados { get; set; } = new List<Juez>();

        public int CategoriaId { get; set; }

        public bool EstaCerrada { get; set; } = false;
        public bool ResultadosPublicados { get; set; } = false;
        public bool RestriccionVotoUnico { get; set; } = false;
        public bool PermiteAutoVoto { get; set; } = false;

        //Propiedades Visibilidad, permisos y configuración
        public bool MostrarNombresJueces { get; set; } = true;
        public bool MostrarComentarios { get; set; } = true;
        public bool MostrarRanking { get; set; } = true;
        public bool MostrarResultadosDetallados { get; set; } = true;


        //Notis
        public bool NotificacionRecordatorioEnviada { get; set; } = false;
        public bool NotificacionCierreEnviada { get; set; } = false;

        private IVotacionState? _state;

        internal IVotacionState GetState()
        {
            if (_state == null)
                SincronizarEstado();
            return _state!;
        }

        internal void SetState(IVotacionState state)
        {
            _state = state;
            Estado = state.Tipo;
        }

        private void SincronizarEstado()
        {
            _state = Estado switch
            {
                EstadoVotacion.Programada => new ProgramadaState(),
                EstadoVotacion.Abierta => new AbiertaState(),
                EstadoVotacion.Cerrada => new CerradaState(),
                EstadoVotacion.Pausada => new PausadaState(),
                EstadoVotacion.ResultadosPublicados => new ResultadosPublicadosState(),
                _ => new ProgramadaState()
            };
        }

        public void CerrarVotacion()
        {
            GetState().Cerrar(this);
        }

        public void CompartirResultados()
        {
            GetState().PublicarResultados(this);
        }

        public bool EnviarNotificacionApertura { get; set; } = true;
        public bool NotificacionAperturaEnviada { get; set; } = false;

        public void ConfigurarFechas(DateTime apertura, DateTime cierre)
        {
            if (apertura.Kind != DateTimeKind.Utc) apertura = apertura.ToUniversalTime();
            if (cierre.Kind != DateTimeKind.Utc) cierre = cierre.ToUniversalTime();

            if (apertura >= cierre)
                throw new ArgumentException("La fecha de cierre debe ser estrictamente posterior a la fecha de apertura.");

            FechaApertura = apertura;
            FechaCierre = cierre;

            EvaluarEstadoTemporal(DateTime.UtcNow);
        }

        public void EvaluarEstadoTemporal(DateTime ahoraUtc)
        {
            GetState().EvaluarTemporal(this, ahoraUtc);
        }

        public bool PuedeVotar(DateTime ahoraUtc)
        {
            return GetState().PuedeVotar(ahoraUtc, this);
        }

        public void ForzarApertura()
        {
            GetState().Abrir(this);
        }

        public void ForzarProgramada()
        {
            GetState().Programar(this);
        }

        public void ForzarCierre()
        {
            GetState().CerrarManual(this);
        }

        public void PausarVotacion()
        {
            GetState().Pausar(this);
        }

        public void ReanudarVotacion()
        {
            GetState().Reanudar(this);
        }
    }
}
