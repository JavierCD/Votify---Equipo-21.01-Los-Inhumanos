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
    public abstract class VotacionEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime FechaCierre { get; set; }
        public string Estado { get; set; } = "Cerrada";

        //Relación con Evento
        public int EventoId { get; set; }
        public virtual EventoEntity Evento { get; set; } = null!;


        // Relación con Categoría
        public int CategoriaId { get; set; }
        public virtual CategoriaEntity Categoria { get; set; } = null!;
        public virtual ICollection<VotoEntity> Votos { get; set; } = new List<VotoEntity>();
    }
}
