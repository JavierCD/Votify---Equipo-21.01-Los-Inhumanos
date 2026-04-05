using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models
{
   public class VotacionPopularDisponibleResponse
    {
        public int VotacionId { get; set; }
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public int MaxSelection { get; set; }
        public List<ProyectoVotacionPopularResponse> Proyectos { get; set; } = new();
    }
}
