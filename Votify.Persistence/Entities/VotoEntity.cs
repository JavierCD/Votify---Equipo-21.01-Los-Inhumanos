using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Votify.Core.Models;
using Votify.Core.Interfaces;

namespace Votify.Persistence.Entities
{
    public class VotoEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public bool Anonimo { get; set; }
        public string? HashAnonimo { get; set; }

        public int? JuezId { get; set; }
        public virtual JuezEntity? Juez { get; set; }

        public int? VotanteId { get; set; }
        public virtual VotanteEntity? Votante { get; set; }

        public int VotacionId { get; set; }
        public virtual VotacionEntity Votacion { get; set; } = null!;
    }
}
