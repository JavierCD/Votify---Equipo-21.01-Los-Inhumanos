using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Models
{
    public abstract class Proyecto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

            // Asignamos la fecha actual por defecto para no tener que recordarlo al crear
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public bool Visible { get; set; }

            // Clave foránea: El proyecto le pertenece a un participante
        public int ParticipanteId { get; set; }
        public Participante? Participante { get; set; }

            // Propiedad de navegación MUCHOS A MUCHOS
            // Un proyecto puede estar nominado a varias categorías, y una categoría tiene varios proyectos
        public List<Categoria> Categorias { get; set; } = new List<Categoria>();

        public List<Voto> Votos { get; set; } = new List<Voto>();

            //Atributos que se asocian con la construcción de la clase para el factory method
        public double CriterioA {  get; set; }
        public double CriterioB { get; set; }

        public Proyecto() { }

        public Proyecto(string nombre, int idParticipante, double criterioA = 0, double criterioB = 0, bool visible = true, string? desc = null)
        {
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new ArgumentException("El nombre del proyecto no puede estar vacío", nameof(nombre));

                if (idParticipante <= 0)
                    throw new ArgumentException("El proyecto debe estar vinculado a un participante válido.", nameof(idParticipante));

                Name = nombre;
            ParticipanteId = idParticipante;
            CriterioA = criterioA;
            CriterioB = criterioB;
                Visible = visible;
                Description = desc;
        }

        public void AgregarCategoria(Categoria categoria)
        {
                if (categoria == null)
                    throw new ArgumentNullException(nameof(categoria), "La categoría no puede ser nula.");

                // Evitamos que se añada la misma categoría dos veces
                if (!Categorias.Contains(categoria))
                {
                    Categorias.Add(categoria);
                }
        }

        public abstract double CalcularPuntuacion();

        public abstract string CategoriaEspecialidad();
    }
}
