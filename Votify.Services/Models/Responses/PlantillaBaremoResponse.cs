using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models.Responses
{
    public class PlantillaBaremoResponse
    {
        public String Id { get; set; } // El ID real de la base de datos
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;

        public List<CriterioResponse> Criterios { get; set; } = new();

        // Propiedad calculada útil para la UI (ej. validar que sumen 100%)
        public int PesoTotal => Criterios.Sum(c => c.Peso);
    }
}
