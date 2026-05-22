using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models.Responses
{
    public class SintesisIAResponse
    {
        public string PuntosFuertes { get; set; } = string.Empty;
        public string AreasMejora { get; set; } = string.Empty;
        public string ConsensoGeneral { get; set; } = string.Empty;
        public DateTime FechaGeneracion { get; set; }
        public bool Existe { get; set; }
    }
}
