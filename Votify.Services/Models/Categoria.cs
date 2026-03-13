using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models
{
    public class Categoria{
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Descripcion { get; set; }
        public List<Proyecto>? Proyectos { get; set; }
        public List<Premio>? Premios { get; set; }
        public Votacion? Votacion { get; set; }
        public Evento? Evento { get; set; }

    }
}
