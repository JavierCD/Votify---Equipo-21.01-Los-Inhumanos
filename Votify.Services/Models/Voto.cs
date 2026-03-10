using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models
{
    internal class Voto{
        public int Id { get; set; }
        public DateTime fecha { get; set; }
        public bool anonimo { get; set; }
        public string hashanonimo { get; set; }

    }
}
