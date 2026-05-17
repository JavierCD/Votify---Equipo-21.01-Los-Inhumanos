using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Models
{
    public class ResultadoIntervenido
    {
        public int Id { get; set; }
        public int VotacionId { get; set; }
        public Votacion Votacion { get; set; } = null!;
        public int ProyectoId { get; set; }
        public Proyecto Proyecto { get; set; } = null!;
        public int Posicion { get; set; }
        public double PuntajeOriginal { get; set; }  
        public DateTime FechaIntervencion { get; set; } = DateTime.UtcNow;
    }
}
