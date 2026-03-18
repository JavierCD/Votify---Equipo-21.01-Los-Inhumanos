using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Models
{
    public class Evento
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        // Mantenemos el valor por defecto que tenías, es una gran práctica
        public string Estado { get; set; } = "Borrador";

        // Clave foránea del organizador (fundamental para el rendimiento)
        public int OrganizadorId { get; set; }
        public Organizador? Organizador { get; set; }

        // Propiedades de navegación (Usando tus listas refinadas)
        public List<Participante> Participantes { get; set; } = new List<Participante>();
        public List<Juez> Jurado { get; set; } = new List<Juez>();
        public List<Votante> Votantes { get; set; } = new List<Votante>();
        public List<Categoria> CategoriasEvento { get; set; } = new List<Categoria>();
    }
}
