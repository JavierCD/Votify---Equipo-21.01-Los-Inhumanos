using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models.Responses
{
    public class ResultadoIntervenidoResponse
    {
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; } = string.Empty;
        public int VotacionId { get; set; }
        public string EstadoVotacion { get; set; } = string.Empty;
        public bool TieneIntervencion { get; set; }  // true si ya se guardó antes
        public List<ProyectoResultadoResponse> Proyectos { get; set; } = new();
        public List<ProyectoResultadoResponse> ProyectosDisponibles { get; set; } = new(); // Los que NO están en el ranking (para "Agregar")

    }
}
