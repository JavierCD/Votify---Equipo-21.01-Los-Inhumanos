using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models
{
    internal class Proyecto{
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime fechaRegistro { get; set; }
        public bool visible { get; set; }

    }
}
