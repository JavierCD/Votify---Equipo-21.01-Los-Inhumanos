using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Models
{
    public abstract class Votacion
    {
        public int Id { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime FechaCierre { get; set; }

        // Mantenemos tu valor por defecto que es muy buena práctica
        public string Estado { get; set; } = "Cerrada";

        // Relación 1 a 1 con Categoría (la clave foránea vive aquí)
        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        // Propiedad de navegación: Una votación tiene muchos votos
        public List<Voto> Votos { get; set; } = new List<Voto>();
    }
}
