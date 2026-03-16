using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Models
{
    public class Votante{
        public int Id { get; set; }
        public string Email { get; set; }

        public bool Anonimo { get; set; }
        public List<Evento> Eventos { get; set; }
        public Voto Voto { get; set; }

    }
}
