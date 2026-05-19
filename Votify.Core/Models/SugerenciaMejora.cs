namespace Votify.Core.Models
{
    public class SugerenciaMejora
    {
        public int Prioridad { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string AccionRecomendada { get; set; } = string.Empty;
    }
}
