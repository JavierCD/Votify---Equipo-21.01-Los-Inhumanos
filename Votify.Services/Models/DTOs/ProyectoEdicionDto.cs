using System;

namespace Votify.Services.DTOs
{
    public class ProyectoEdicionDto
    {
        // Datos para lógica y actualización
        public int Id { get; set; }
        public int ParticipanteId { get; set; }

        // Datos editables por el usuario
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