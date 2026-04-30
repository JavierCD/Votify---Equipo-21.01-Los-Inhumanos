using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models.DTOs
{
    public class PosicionRankingDto
    {
        public int Posicion { get; set; }
        public string NombreProyecto { get; set; } = string.Empty;
        public double PuntosTotales { get; set; }
        public string Medalla => Posicion == 1 ? "🥇 " : Posicion == 2 ? "🥈 " : Posicion == 3 ? "🥉 " : "";
    }
}
