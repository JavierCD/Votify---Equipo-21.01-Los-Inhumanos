using Votify.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models.Responses
{
   public class PopularResponse
    {
        public int Id { get; set; }
        public int CategoriaId { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime FechaCierre { get; set; }
        public EstadoVotacion Estado { get; set; }
        public int MaxSelection { get; set; }
        public bool PermiteAutoVoto { get; set; }
    }
}
