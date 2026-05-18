namespace Votify.Core.Models
{
    public class HojaRutaMejora
    {
        public int ProyectoId { get; set; }
        public string ProyectoNombre { get; set; } = string.Empty;
        public int TotalComentariosAnalizados { get; set; }
        public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;
        public List<SugerenciaMejora> Sugerencias { get; set; } = new();
    }
}
