using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Models
{
    public class Evento{
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string estado { get; set; }
        public List<Participante> Participante { get; set; }
        public List<Juez> Jurado { get; set; }
        public Organizador Organizador { get; set; }
        public List<Votante> Votantes { get; set; }
        public List<Categoria> CategoriasEvento { get; set; }
    }
}
