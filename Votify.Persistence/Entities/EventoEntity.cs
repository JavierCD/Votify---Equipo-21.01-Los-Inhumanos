using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Persistence.Entities
{
    public class EventoEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public string? Descripcion { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }
        
        [Required]
        public DateTime FechaFin {  get; set; }

        //Relaciones
        //public virtual ICollection<>

    }
}
