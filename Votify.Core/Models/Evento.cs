using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Votify.Core.Enums;

namespace Votify.Core.Models
{
    public abstract class Evento
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public string CodigoAcceso { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public EstadoEvento Estado { get; set; } = EstadoEvento.Borrador;

        // Clave foránea del organizador (fundamental para el rendimiento)
        public int OrganizadorId { get; set; }
        public Organizador? Organizador { get; set; }

        // Propiedades de navegación (Usando tus listas refinadas)
        public List<Participante> Participantes { get; set; } = new List<Participante>();
        public List<Juez> Jurado { get; set; } = new List<Juez>();
        public List<Votante> Votantes { get; set; } = new List<Votante>();
        public List<Categoria> CategoriasEvento { get; set; } = new List<Categoria>();

        public Evento() { }

        public Evento(string name, DateTime fechaInit, DateTime fechaFin, int orgaId, string? desc = null) 
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("El nombre del evento es obligatorio.", nameof(name));

            if (fechaFin <= fechaInit) throw new ArgumentException("La fecha de acabar el evento no puede ser anterior a la fecha de inicio.");

            if(orgaId <= 0) throw new ArgumentException("El Id del organizador es inválido.", nameof(OrganizadorId));

            Name = name;
            FechaFin = fechaFin;
            FechaInicio = fechaInit;
            OrganizadorId = orgaId;
            Description = desc;
            Estado = EstadoEvento.Borrador;
            CodigoAcceso = GenerarCodigoAcceso();
        }

        private string GenerarCodigoAcceso()
        {
            var chars = "ABCDEFGIJKLMNOPQRSTUWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6).Select(s=> s[random.Next(s.Length)]).ToArray());
        }

        public void ModificarVentanaDeTiempo(DateTime nuevaInicio, DateTime nuevaFim)
        {
            if(nuevaFim <= nuevaInicio) throw new ArgumentException("La fecha de acabar el evento no puede ser anterior a la fecha de inicio.");

            FechaInicio = nuevaInicio;
            FechaFin = nuevaFim;
        }

        public void ActualizarDetalles(string newName, string? desc = null)
        {
            if (string.IsNullOrWhiteSpace(newName)) throw new ArgumentNullException("El nombre del evento es obligatorio.", nameof(newName));
            
            Name = newName;
            Description = desc;
        }

        public void PublicarEvento()
        {
            if (Estado != EstadoEvento.Borrador) throw new InvalidOperationException("Solo se pueden publicar Eventos que están en estado de Borrador");

            Estado = EstadoEvento.Activo;
        }

        public void AgregarCategoria(string nombreCat, string? descCat = null)
        {
            if(CategoriasEvento.Any(c  => c.Name.Equals(nombreCat, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"El evento ya contiene una Categoría llamada '{nombreCat}'.");

            var nuevaCat = new Categoria(nombreCat, descCat); 

            CategoriasEvento.Add(nuevaCat);


        }

        public abstract string Modalidad();
    }
}
