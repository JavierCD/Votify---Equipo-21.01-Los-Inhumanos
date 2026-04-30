using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models
{
    public class AuditoriaVotoResponse
    {
        public int VotoId { get; set; }
        public DateTime FechaEmision { get; set; }
        public string ProyectoNombre { get; set; } = string.Empty;
        public string TipoVotacion { get; set; } = string.Empty;
        public string IdentificadorVotante { get; set; } = string.Empty;
        public bool EsAnonimo { get; set; }
    }
}
