using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models.DTOs
{
    public record CriterioDto {
        public string Nombre { get; set; }
        public int Peso { get; set; }
    }

    public class PlantillaBaremoDto
    {
        public string Id { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public List<CriterioDto> Criterios { get; set; } = new();
    }
}
