using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models.Requests
{
  public class GuardarResultadoRequest
    {
        public int ProyectoId { get; set; }
        public int Posicion { get; set; }
        public double PuntajeOriginal { get; set; }
    }
}
