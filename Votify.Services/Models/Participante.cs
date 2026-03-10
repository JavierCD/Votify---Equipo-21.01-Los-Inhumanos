using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models
{
    internal class Participante : Miembro{
        public string Descripcion { get; set; }
        public bool visible { get; set; }
        public string estado { get; set; }

    }
}
