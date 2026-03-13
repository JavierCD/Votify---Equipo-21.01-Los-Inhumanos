using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models
{
    public class Premio {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Descripcion { get; set; }
        public int Posicion { get; set; }
        public Categoria Categoria { get; set; }

    }
}
