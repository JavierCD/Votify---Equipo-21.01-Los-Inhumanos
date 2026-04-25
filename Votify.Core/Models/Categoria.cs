using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models;

namespace Votify.Core.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Descripcion { get; set; }

        // Clave foránea (es pragmático y muy útil dejarla en el dominio)
        public int EventoId { get; set; }

        // Propiedades de navegación puras (Sin la palabra "virtual" ni "Entity")
        public Evento? Evento { get; set; }
        public Votacion? Votacion { get; set; }
        public List<Proyecto> Proyectos { get; set; } = new List<Proyecto>();
        public List<Premio> Premios { get; set; } = new List<Premio>();

        public Categoria() { }

        public Categoria(string name, string? desc)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre de la categoría no puede estar vacío.", nameof(name));

            Name = name;
            Descripcion = desc;
        }

        public void ActualizarDetalles(string newName, string? descripcion = null)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("El nombre de la categoría no puede estar vacío", nameof(newName));

            Name = newName;
            Descripcion = descripcion;
        }

        public void AsignarVotacion(Votacion votacion)
        {
            Votacion = votacion ?? throw new ArgumentNullException(nameof(votacion));
        }

        public void AsignarPremio(string nombre, string descripcion, int puesto)
        {
            if (puesto <= 0)
                throw new ArgumentException("El puesto debe ser mayor a cero.");

            if (Premios.Any(p => p.Posicion == puesto))
                throw new InvalidOperationException($"Ya existe un premio para el puesto {puesto}.");

            var nuevoPremio = new Premio(nombre, descripcion, puesto);

            Premios.Add(nuevoPremio);
        }

        public void EliminarPremio(Premio premio)
        {
            if(premio!=null && Premios.Contains(premio))
            {
                Premios.Remove(premio);
            }
        }
    }
}
