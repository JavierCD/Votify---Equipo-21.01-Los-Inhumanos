using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models
{
    public class EmitirVotoMulticriterioRequest
    {
        public int VotacionId { get; set; }
        public string Email { get; set; } = string.Empty;

        // Diccionario anidado: ProyectoId -> (CriterioId -> Puntuacion)
        public Dictionary<int, Dictionary<int, int>> Puntuaciones { get; set; } = new();
    }
}
