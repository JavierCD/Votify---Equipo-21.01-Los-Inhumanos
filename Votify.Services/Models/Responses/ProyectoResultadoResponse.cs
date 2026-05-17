using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models.Responses
{
    public class ProyectoResultadoResponse
    {
        public int ProyectoId { get; set; }
        public string NombreProyecto { get; set; } = string.Empty;
        public string NombreEquipo { get; set; } = string.Empty;  // Participante.Name
        public double Puntaje { get; set; }
        public int Posicion { get; set; }
    }
}
