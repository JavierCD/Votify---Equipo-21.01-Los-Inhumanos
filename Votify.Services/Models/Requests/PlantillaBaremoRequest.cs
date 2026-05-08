using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models.Requests
{
    public class PlantillaBaremoRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;

        // Contiene una lista de Requests, no de Responses ni DTOs genéricos
        public List<CriterioRequest> Criterios { get; set; } = new();
    }
}
