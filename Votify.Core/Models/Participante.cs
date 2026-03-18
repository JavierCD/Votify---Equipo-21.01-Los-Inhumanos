using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models.Votify.Core.Models;

namespace Votify.Core.Models
{
    public class Participante : Miembro
    {
        // La dejamos nullable porque las descripciones suelen ser opcionales
        public string? Descripcion { get; set; }

        public bool Visible { get; set; }

        // Mantenemos tu excelente valor por defecto
        public string Estado { get; set; } = "Pendiente";

        // Propiedad de navegación pura
        public Proyecto? Proyecto { get; set; }
    }
}
