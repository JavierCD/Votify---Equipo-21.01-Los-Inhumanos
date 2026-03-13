using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models
{
    public abstract class Votacion {
        public int Id { get; set; }
        public DateTime fechaApertura { get; set; }
        public DateTime fechaCierre { get; set; }
        public string estado { get; set; }
        public List<Voto> Votos { get; set; }
        public Categoria Categoria { get; set; }


    }
}
