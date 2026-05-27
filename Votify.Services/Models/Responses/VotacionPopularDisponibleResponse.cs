using Votify.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models.Responses
{
   public class VotacionPopularDisponibleResponse
    {
        public int VotacionId { get; set; }
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; } = string.Empty;
        public EstadoVotacion Estado { get; set; }
        public int MaxSelection { get; set; }
        public bool PermiteAutoVoto { get; set; }
        public int ParticipanteId { get; set; }
        public List<ProyectoVotacionPopularResponse> Proyectos { get; set; } = new();
    }
}
