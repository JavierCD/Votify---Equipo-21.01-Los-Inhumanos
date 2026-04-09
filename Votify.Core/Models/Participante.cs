using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models;

namespace Votify.Core.Models
{
    public class Participante : Miembro
    {
        // La dejamos nullable porque las descripciones suelen ser opcionales
        public string? Descripcion { get; set; }

        public bool Visible { get; set; }

        // Mantenemos tu excelente valor por defecto
        public string Estado { get; set; } = "Pendiente";

        // Propiedad de navegación pura
        public Proyecto? Proyecto { get; set; }

        // Constructor vacío necesario para Entity Framework
        public Participante():base() { }

        public Participante(string name, string email, string password):base(name, email, password)
        {

        }

        // --- MÉTODOS DE DOMINIO (Reglas de negocio) ---

        public void ActualizarFicha(string? nuevaDescripcion)
        {
            // Aquí centralizamos las reglas. Por ejemplo, si no queremos descripciones larguísimas:
            if (nuevaDescripcion?.Length > 500)
                throw new ArgumentException("La descripción no puede superar los 500 caracteres.");

            Descripcion = nuevaDescripcion;
        }

        public void CambiarVisibilidad(bool hacerVisible)
        {
            // Aquí podríamos meter lógica futura, ej: "No se puede hacer visible si no tiene proyecto"
            Visible = hacerVisible;
        }

        public void EvaluarEstado(string nuevoEstado)
        {
            if (string.IsNullOrWhiteSpace(nuevoEstado))
                throw new ArgumentNullException(nameof(nuevoEstado), "El estado es obligatorio.");

            // Blindamos la clase para que nadie se invente estados raros
            var estadosValidos = new[] { "Pendiente", "Aprobado", "Rechazado" };
            if (!Array.Exists(estadosValidos, e => e.Equals(nuevoEstado, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"El estado '{nuevoEstado}' no es un estado válido.");
            }

            Estado = nuevoEstado;
        }
    }
    //Prueba verificación rama github y cambio nombre rama para que salga bien en github 
}

