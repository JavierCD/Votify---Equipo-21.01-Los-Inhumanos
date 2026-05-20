using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Models
{
    public class SintesisIA
    {
        public int Id { get; set; }
        public int ProyectoId { get; set; }
        public Proyecto Proyecto { get; set; } = null!;
        public string PuntosFuertes { get; set; } = string.Empty;
        public string AreasMejora { get; set; } = string.Empty;
        public string ConsensoGeneral { get; set; } = string.Empty;
        public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;
    }
}
