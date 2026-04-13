using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Models
{
    public class Notificacion
    {
        public int Id { get; set; }

        // A quién va dirigida
        public int MiembroId { get; set; }
        public Miembro Miembro { get; set; } = null!;

        // Contenido
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;

        // Enlace directo a la evaluación (Criterio de Aceptación 5)
        public string UrlAccion { get; set; } = string.Empty;

        // Estado
        public bool Leida { get; set; } = false;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Constructor para EF
        public Notificacion() { }

        public Notificacion(int miembroId, string titulo, string mensaje, string urlAccion)
        {
            MiembroId = miembroId;
            Titulo = titulo;
            Mensaje = mensaje;
            UrlAccion = urlAccion;
        }

        public void MarcarComoLeida()
        {
            Leida = true;
        }
    }
}
