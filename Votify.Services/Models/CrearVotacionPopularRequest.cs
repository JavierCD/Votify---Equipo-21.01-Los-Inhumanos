using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models
{
    internal class CrearVotacionPopularRequest
    {
        public int EventoId { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime FechaCierre { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int MaxSelecciones { get; set; }
    }
}
