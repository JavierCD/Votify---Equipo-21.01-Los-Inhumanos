using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Models
{
    public class Proyecto{
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Visible { get; set; }
        public Participante Participante { get; set; }
        public List<Categoria> Categorias { get; set; }


    }
}
