using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Persistence.Entities
{
    internal class Evento
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "El nombre del evento es obligatorio")]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }
        [Required]
        public DateTime FechaFin { get; set; }

        //public ICollection<Participante> 

        public DateTime FechaCreacion { get; set; }
    }
}
