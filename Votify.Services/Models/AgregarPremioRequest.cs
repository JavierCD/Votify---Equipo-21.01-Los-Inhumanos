using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models
{
    public class AgregarPremioRequest
    {
        public string nombrePremio { get; set; }
        public string premioDesc {  get; set; }
        public int puesto { get; set; }
        public int categoriaID { get; set; }
        public bool PermiteEmpate { get; set; }

    }
}
