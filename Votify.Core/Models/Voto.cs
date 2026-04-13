using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models;

namespace Votify.Core.Models
{
    public abstract class Voto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public bool Anonimo { get; set; }
        public string? HashAnonimo { get; set; }
        public string? Comentario { get; set; }

        public double PuntuacionBase { get; set; }

        // La votación en la que se emite (Obligatorio)
        public int? VotanteId { get; set; }
        public virtual Votante? Votante { get; set; }
        public int VotacionId { get; set; }
        public Votacion? Votacion { get; set; }

        // ---> ¡LO NUEVO! El proyecto al que se le da el voto (Obligatorio) <---
        public int ProyectoId { get; set; }
        public Proyecto? Proyecto { get; set; }

        protected Voto() { }

        protected Voto(int votacionId, int proyectoId, double puntuacionBase, bool anonimo = false, string? hashAnonimo = null,string? comentario=null)
        {
            VotacionId = votacionId;
            ProyectoId = proyectoId;
            PuntuacionBase = puntuacionBase;
            Anonimo = anonimo;
            HashAnonimo = hashAnonimo;
            Comentario = comentario;
            Fecha = DateTime.UtcNow;
        }

        public void AssignId(int id)
        {
            VotanteId = id;
        }

        public abstract double CalcularPuntuacionNormalizada(); 
        public abstract string RolVotante();

    }
}
