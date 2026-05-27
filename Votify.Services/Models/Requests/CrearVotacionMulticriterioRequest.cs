using Votify.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Services.Models.Requests;

namespace Votify.Services.Models.Requests
{
    public class CrearVotacionMulticriterioRequest
    {
        public int CategoriaId { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime FechaCierre { get; set; }
        public EstadoVotacion Estado { get; set; } = EstadoVotacion.Abierta;
        public bool EnviarNotificacionApertura { get; set; }
        public List<CriterioRequest> Criterios { get; set; } = new();
        public bool PermiteAutoVoto { get; set; } = false;
        public bool RestriccionVotoUnico { get; set; } = false;
    }
}
