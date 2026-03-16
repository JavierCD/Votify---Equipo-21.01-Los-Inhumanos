using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Votify.Core.Models;
using Votify.Core.Interfaces;

namespace Votify.Persistence.Entities
{
    public class CategoriaEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        public int EventoId { get; set; }
        public virtual EventoEntity Evento { get; set; } = null!;
        public virtual VotacionEntity? Votacion { get; set; }
        public virtual ICollection<ProyectoEntity> Proyectos { get; set; } = new List<ProyectoEntity>();
        public virtual ICollection<PremioEntity> Premios { get; set; } = new List<PremioEntity>();
    }
}
