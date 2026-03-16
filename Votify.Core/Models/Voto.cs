using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Models
{
    public class Voto{
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public bool Anonimo { get; set; }
        public string HashAnonimo { get; set; }

        public Juez Juez { get; set; }
        public Votante Votante { get; set; }
        public Votacion Votacion { get; set; }

    }
}
