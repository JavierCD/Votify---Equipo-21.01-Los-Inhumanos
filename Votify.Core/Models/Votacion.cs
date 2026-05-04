using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Models
{
    public abstract class Votacion
    {
        public int Id { get; set; }
        public int EventoId { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime FechaCierre { get; set; }
        public string Estado { get; set; }
        public List<Voto> Votos { get; set; }
        public Categoria Categoria { get; set; }
        public virtual ICollection<Juez> JuecesAutorizados { get; set; } = new List<Juez>();

        // Relación 1 a 1 con Categoría (la clave foránea vive aquí)
        public int CategoriaId { get; set; }
        //public Categoria? Categoria { get; set; }

        public bool EstaCerrada { get; set; } = false;
        public bool ResultadosPublicados { get; set; } = false;
        public bool RestriccionVotoUnico { get; set; } = false;
        public bool PermiteAutoVoto { get; set; } =false;

        //Notis
        public bool NotificacionRecordatorioEnviada { get; set; } = false;
        public bool NotificacionCierreEnviada { get; set; } = false;

        public void CerrarVotacion()
        {
            if (EstaCerrada) throw new InvalidOperationException("La votación ya está cerrada");
            EstaCerrada = true;
            // hacer un enum para cambiar la variable estado de string a enum
        }

        public void CompartirResultados()
        {
            if(!EstaCerrada) throw new InvalidOperationException("No puedes publicar resultados de una votación que está abierta");
            ResultadosPublicados = true;
            Estado = "ResultadosPublicados";
        }

        // El organizador puede decidir si esta votación en concreto hace "ruido" o no al abrirse
        public bool EnviarNotificacionApertura { get; set; } = true;
        // Bandera de control interno
        public bool NotificacionAperturaEnviada { get; set; } = false;

        public void ConfigurarFechas(DateTime apertura, DateTime cierre)
        {
            // Las bases de datos y .NET trabajan mejor con UTC.
            if (apertura.Kind != DateTimeKind.Utc) apertura = apertura.ToUniversalTime();
            if (cierre.Kind != DateTimeKind.Utc) cierre = cierre.ToUniversalTime();

            if (apertura >= cierre)
                throw new ArgumentException("La fecha de cierre debe ser estrictamente posterior a la fecha de apertura.");

            FechaApertura = apertura;
            FechaCierre = cierre;

            // Reevaluamos el estado inmediatamente
            EvaluarEstadoTemporal(DateTime.UtcNow);
        }

        public void EvaluarEstadoTemporal(DateTime ahoraUtc)
        {
            // Si un admin la cerró o pausó a la fuerza, respetamos ese estado manual
            if (Estado == "CerradaManual" || Estado == "Pausada")
                return;

            if (ahoraUtc < FechaApertura)
            {
                Estado = "Programada";
            }
            else if (ahoraUtc >= FechaApertura && ahoraUtc <= FechaCierre)
            {
                Estado = "Abierta";
            }
            else if (ahoraUtc > FechaCierre)
            {
                Estado = "Cerrada";
            }
        }

        public bool PuedeVotar(DateTime ahoraUtc)
        {
            EvaluarEstadoTemporal(ahoraUtc); // Lazy evaluation (evaluación perezosa)
            return Estado == "Abierta";
        }

        // Métodos para forzar estados manualmente
        public void ForzarApertura()
        {
            Estado = "Abierta";
            EstaCerrada = false;
            FechaApertura = DateTime.UtcNow; // Cambiamos la fecha de apertura a AHORA

            // Si la fecha de cierre original ya había pasado, le damos 1 día más por defecto
            if (FechaCierre <= FechaApertura)
            {
                FechaCierre = FechaApertura.AddDays(1);
            }
        }

        public void ForzarProgramada()
        {
            Estado = "Programada";
            EstaCerrada = false;

            // Si ya estaba abierta, forzamos que abra mañana para que tenga sentido
            if (FechaApertura <= DateTime.UtcNow)
            {
                FechaApertura = DateTime.UtcNow.AddDays(1);
            }
            if (FechaCierre <= FechaApertura)
            {
                FechaCierre = FechaApertura.AddDays(1);
            }
        }

        public void ForzarCierre()
        {
            Estado = "CerradaManual";
            EstaCerrada = true;
            FechaCierre = DateTime.UtcNow; // Ajustamos el cierre a AHORA
        }

        public void PausarVotacion() => Estado = "Pausada";

        public void ReanudarVotacion()
        {
            Estado = ""; // Limpiamos estado manual
            EvaluarEstadoTemporal(DateTime.UtcNow);
        }

    }
}
