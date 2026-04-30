using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models
{
    public class CrearEventoRequest
    {
        [Required(ErrorMessage = "El nombre del evento es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El tipo de evento es obligatorio")]
        public string TipoEvento { get; set; } = "Hackathon"; // Valor por defecto

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        public DateTime FechaInicio { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        public DateTime FechaFin { get; set; } = DateTime.Today.AddDays(7);

        // Aquí guardaremos las categorías dinámicas
        public List<CategoriaViewModel> Categorias { get; set; } = new List<CategoriaViewModel>();
    }
}
