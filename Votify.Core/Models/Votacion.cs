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
        }

        // El organizador puede decidir si esta votación en concreto hace "ruido" o no al abrirse
        public bool EnviarNotificacionApertura { get; set; } = true;
        // Bandera de control interno
        public bool NotificacionAperturaEnviada { get; set; } = false;
       
    }
}
