using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Models
{
    public class Multicriterio : Votacion
    {
        public bool UsaPesos { get; set; }

        // Rescatamos la lista de criterios de tu antigua Entity
        // porque el dominio necesita conocer los criterios que lo componen
        public List<Criterio> Criterios { get; set; } = new List<Criterio>();
    }
}
