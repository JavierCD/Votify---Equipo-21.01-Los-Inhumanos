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
    public class CriterioEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Corregido de Naame del txt
        public string? Descripcion { get; set; }
        public float Peso { get; set; }

        public int MulticriterioId { get; set; }
        public virtual MulticriterioEntity Votacion { get; set; } = null!;
    }
}
