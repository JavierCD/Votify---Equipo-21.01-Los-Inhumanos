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

        // Relación 1 a 1 con Categoría (la clave foránea vive aquí)
        public int CategoriaId { get; set; }
        //public Categoria? Categoria { get; set; }

        // El organizador puede decidir si esta votación en concreto hace "ruido" o no al abrirse
        public bool EnviarNotificacionApertura { get; set; } = true;

        // Bandera de control interno (¡MUY IMPORTANTE!)
        // Para que nuestro "Vigilante" no envíe 50 correos/notificaciones por minuto si la fecha coincide
        public bool NotificacionAperturaEnviada { get; set; } = false;

        // Propiedad de navegación: Una votación tiene muchos votos
        //public List<Voto> Votos { get; set; } = new List<Voto>();
    }
}
