using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Votify.Core.Models
{
    public class Participante : Miembro
    {
        public string? Descripcion { get; set; }
        public string? InstitucionEducativa { get; set; }
        public string? Intereses { get; set; }
        public string ColorFondo { get; set; } = "#479cc8";
        public string? UrlFoto { get; set; }

        public ICollection<Proyecto> Proyectos { get; set; } = new List<Proyecto>();

        public Participante() : base() { }

        public Participante(string name, string email, string password) : base(name, email, password)
        {

        }

        public void ActualizarFicha(string? nuevaDescripcion)
        {
            if (nuevaDescripcion?.Length > 500)
                throw new ArgumentException("La descripción no puede superar los 500 caracteres.");

            Descripcion = nuevaDescripcion;
        }
        public IEnumerable<Proyecto> ObtenerProyectosParaEvento(Evento evento)
        {
            if (Proyectos == null || evento?.CategoriasEvento == null)
            {
                return new List<Proyecto>();
            }

            // Toda la lógica de negocio (LINQ) se encapsula en el Core, 
            // haciéndola 100% testeable con xUnit.
            return Proyectos.Where(p =>
                p.Categorias != null &&
                p.Categorias.Any(catProyecto =>
                    evento.CategoriasEvento.Any(catEvento => catEvento.Id == catProyecto.Id)
                )
            ).ToList();
        }
    }
}

