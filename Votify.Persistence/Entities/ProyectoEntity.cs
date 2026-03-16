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
    public class ProyectoEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public bool Visible { get; set; }

        public int ParticipanteId { get; set; }
        public virtual ParticipanteEntity Participante { get; set; } = null!;
        public virtual ICollection<CategoriaEntity> Categorias { get; set; } = new List<CategoriaEntity>();
    }
}
