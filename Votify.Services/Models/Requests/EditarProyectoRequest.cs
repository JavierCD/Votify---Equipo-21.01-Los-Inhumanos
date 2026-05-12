using System;

namespace Votify.Services.Models.Requests
{
    public class EditarProyectoRequest
    {
        public int Id { get; set; } // Necesario para saber cuál actualizar
        public int ParticipanteId { get; set; } // Opcional, por seguridad para saber quién edita

        // Solo lo que el usuario realmente puede cambiar
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? NombresEquipo { get; set; }
        public string? UrlMateriales { get; set; }
    }
}