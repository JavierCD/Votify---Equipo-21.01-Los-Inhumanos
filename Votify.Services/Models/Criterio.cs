using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models
{
    internal class Criterio{
        public int Id { get; set; }
        public string Naame { get; set; }
        public string Descripcion { get; set; }
        public float peso { get; set; }

    }
}
