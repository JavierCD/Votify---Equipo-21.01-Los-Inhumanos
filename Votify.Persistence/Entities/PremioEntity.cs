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
    public class PremioEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int Posicion { get; set; }

        public int CategoriaId { get; set; }
        public virtual CategoriaEntity Categoria { get; set; } = null!;
    }
}
