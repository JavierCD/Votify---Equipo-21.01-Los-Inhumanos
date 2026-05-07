using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models.Requests
{
    public class CriterioRequest
    {
        // Un ID temporal SOLO para que Blazor pueda borrar/editar elementos 
        // en la lista de la pantalla antes de guardar en base de datos.
        public string IdTemporalUI { get; set; } = Guid.NewGuid().ToString();

        public string Nombre { get; set; } = string.Empty;
        public int Peso { get; set; }
    }
}
