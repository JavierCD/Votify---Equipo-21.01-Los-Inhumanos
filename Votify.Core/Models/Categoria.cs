using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models.Votify.Core.Models;

namespace Votify.Core.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public string? Descripcion { get; set; }

        // Clave foránea (es pragmático y muy útil dejarla en el dominio)
        public int EventoId { get; set; }

        // Propiedades de navegación puras (Sin la palabra "virtual" ni "Entity")
        public Evento? Evento { get; set; }
        public Votacion? Votacion { get; set; }
        public List<Proyecto> Proyectos { get; set; } = new List<Proyecto>();
        public List<Premio> Premios { get; set; } = new List<Premio>();
    }
}
