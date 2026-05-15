using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models.Responses
{
    public class JuezSupervisionDto
    {
        public int JuezId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool HaVotado { get; set; }
        public string Estado => HaVotado ? "Completado" : "Pendiente";
    }
}
