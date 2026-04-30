using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models
{
    public class PuntuacionResponse
    {
        public int Id { get; set; }
        public int CategoriaId { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime FechaCierre { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int ValorMax { get; set; }
        public bool PermiteAutoVoto { get; set; }
        public bool RestriccionVotoUnico { get; set; }
    }
}
