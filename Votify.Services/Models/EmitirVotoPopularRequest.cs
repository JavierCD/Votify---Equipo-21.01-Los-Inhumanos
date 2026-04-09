using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models
{
    public class EmitirVotoPopularRequest
    {
        public int VotacionId { get; set; }
        public int VotanteId { get; set; }
        public List<int> ProyectosSeleccionadosIds { get; set; } = new();
    }
}
