using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models
{
    public class VotacionMulticriterioDetalleResponse
    {
        public int VotacionId { get; set; }
        public string CategoriaNombre { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public List<ProyectoDto> Proyectos { get; set; } = new();
        public List<CriterioBaremoDto> Criterios { get; set; } = new();

        public class ProyectoDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        public class CriterioBaremoDto
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public int Peso { get; set; }
        }
    }
}
