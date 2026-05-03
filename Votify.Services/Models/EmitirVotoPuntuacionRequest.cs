using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models
{
    public class EmitirVotoPuntuacionRequest
    {
        public int VotacionId { get; set; }
        public int VotanteId { get; set; }
        public int? JuezId { get; set; }
        public bool Anonimo { get; set; } = false;
        public string Email { get; set; }
        public Dictionary<int, int> PuntuacionesPorProyecto { get; set; } = new();
    }
}
