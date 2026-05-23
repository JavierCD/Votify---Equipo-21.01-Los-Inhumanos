using System;
using System.Collections.Generic;
using Votify.Core.Models;

namespace Votify.Core.Models
{
    public class VotoValidationContext
    {
        public int VotacionId { get; set; }
        public int VotanteId { get; set; }
        public int? JuezId { get; set; }
        public string? Email { get; set; }
        public bool Anonimo { get; set; }
        public Votacion? Votacion { get; set; }
        public List<Proyecto>? ProyectosValidos { get; set; }
        public Votante? VotanteResuelto { get; set; }

        // Puntuacion-specific
        public Dictionary<int, int>? PuntuacionesPorProyecto { get; set; }

        // Popular-specific
        public List<int>? ProyectosSeleccionadosIds { get; set; }

        // Multicriterio-specific
        public Dictionary<int, Dictionary<int, int>>? PuntuacionesMulticriterio { get; set; }
    }
}
