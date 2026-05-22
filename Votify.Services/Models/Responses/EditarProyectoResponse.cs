using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models.Responses
{
    public class EditarProyectoResponse
    {
        public int Id { get; set; }
        public int ParticipanteId { get; set; }
        public string CorreoParticipante { get; set; } = string.Empty;
        // Datos actuales para pre-rellenar el formulario
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? NombresEquipo { get; set; }
        public string? UrlMateriales { get; set; }

        // Datos de Solo Lectura (Contexto visual para la UI)
        public string NombreEvento { get; set; } = "Evento no especificado";
        public string NombreCategoria { get; set; } = "Sin categoría";
        public string Especialidad { get; set; } = "No definida";
        public string CorreoAdmin { get; set; } = "admin@evento.com";
    }
}
