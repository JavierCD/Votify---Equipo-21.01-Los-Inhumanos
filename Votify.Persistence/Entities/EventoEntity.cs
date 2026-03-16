using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models;
using Votify.Core.Interfaces;

namespace Votify.Persistence.Entities
{
    public class EventoEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; } = "Borrador";

        // Organizador único
        public int OrganizadorId { get; set; }
        public virtual OrganizadorEntity Organizador { get; set; } = null!;

        // Listas de navegación
        public virtual ICollection<MiembroEntity> Miembros { get; set; } = new List<MiembroEntity>();
        public virtual ICollection<VotanteEntity> Votantes { get; set; } = new List<VotanteEntity>();
        public virtual ICollection<CategoriaEntity> CategoriasEvento { get; set; } = new List<CategoriaEntity>();

    }
}
