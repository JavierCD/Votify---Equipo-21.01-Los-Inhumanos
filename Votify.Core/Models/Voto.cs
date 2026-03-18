using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models.Votify.Core.Models;

namespace Votify.Core.Models
{
    public class Voto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public bool Anonimo { get; set; }
        public string? HashAnonimo { get; set; }

        // El autor del voto (Opcionales: o vota un Juez o un Votante)
        public int? JuezId { get; set; }
        public Juez? Juez { get; set; }

        public int? VotanteId { get; set; }
        public Votante? Votante { get; set; }

        // La votación en la que se emite (Obligatorio)
        public int VotacionId { get; set; }
        public Votacion? Votacion { get; set; }

        // ---> ¡LO NUEVO! El proyecto al que se le da el voto (Obligatorio) <---
        public int ProyectoId { get; set; }
        public Proyecto? Proyecto { get; set; }
    }
}
