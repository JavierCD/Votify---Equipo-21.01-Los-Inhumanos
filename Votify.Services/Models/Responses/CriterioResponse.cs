using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models.Responses
{
    public class CriterioResponse
    {
        public int Id { get; set; } // El ID real de la base de datos
        public string Nombre { get; set; } = string.Empty;
        public int Peso { get; set; }
    }
}
