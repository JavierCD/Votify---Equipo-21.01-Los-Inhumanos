using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Models
{
    public class Criterio
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Descripcion { get; set; }

        public float Peso { get; set; }

        // Clave foránea
        public int MulticriterioId { get; set; }

        // Propiedad de navegación pura
        public Multicriterio? Votacion { get; set; }

        public Criterio() { }
    }
}
