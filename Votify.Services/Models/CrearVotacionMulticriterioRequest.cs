using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Services.Models.DTOs;

namespace Votify.Services.Models
{
    public class CrearVotacionMulticriterioRequest
    {
        public int CategoriaId { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime FechaCierre { get; set; }
        public string Estado { get; set; } = "Abierta";
        public bool EnviarNotificacionApertura { get; set; }
        public List<CriterioDto> Criterios { get; set; } = new();
    }
}
