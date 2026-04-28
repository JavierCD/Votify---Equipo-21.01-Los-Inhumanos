using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Models
{
    public class DetalleVoto
    {
        public int Id { get; set; }

        // Relación con la papeleta general (VotoPublico, VotoExperto, etc.)
        public int VotoId { get; set; }
        public Voto Voto { get; set; } = null!;

        public int ProyectoId { get; set; }
        public Proyecto Proyecto { get; set; } = null!;

        // Es nullable (int?) porque si es una votación "Por Puntuación" simple, no hay criterio.
        // Si es "Multicriterio", este campo tendrá el ID del criterio.
        public int? CriterioId { get; set; }
        public Criterio? Criterio { get; set; }

        public int Puntuacion { get; set; }
    }
}
