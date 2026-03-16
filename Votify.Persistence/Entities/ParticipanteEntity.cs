using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Votify.Core.Models;
using Votify.Core.Interfaces;

namespace Votify.Persistence.Entities
{
    public class ParticipanteEntity : MiembroEntity
    {
        public string? Descripcion { get; set; }
        public bool Visible { get; set; }
        public string Estado { get; set; } = "Pendiente";

        // Relación con Proyecto
        public int? ProyectoId { get; set; }
        public virtual ProyectoEntity? Proyecto { get; set; }
    }
}
