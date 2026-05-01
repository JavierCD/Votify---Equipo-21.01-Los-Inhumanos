using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models
{
    public class EvaluacionJuezResponse
    {
        public string NombreJuez { get; set; } = string.Empty;
        public double Puntuacion { get; set; }
        public string? Comentario { get; set; }
        public DateTime Fecha { get; set; }

    }
}
