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
    public class VotanteEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Email { get; set; } = string.Empty;
        public bool Anonimo { get; set; }
        public virtual ICollection<EventoEntity> Eventos { get; set; } = new List<EventoEntity>();
        public virtual ICollection<VotoEntity> Votos { get; set; } = new List<VotoEntity>();
    }
}
