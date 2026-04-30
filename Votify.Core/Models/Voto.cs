namespace Votify.Core.Models
{
    public abstract class Voto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public bool Anonimo { get; set; }
        public string? HashAnonimo { get; set; }
        public string? Comentario { get; set; }
        public double PuntuacionBase { get; set; }

        public int VotacionId { get; set; }
        public Votacion? Votacion { get; set; }
        public int ProyectoId { get; set; }
        public Proyecto? Proyecto { get; set; }

        public List<DetalleVoto> Detalles { get; set; } = new();

        protected Voto() { }

        protected Voto(int votacionId, int proyectoId, double puntuacionBase, bool anonimo = false, string? hashAnonimo = null, string? comentario = null)
        {
            VotacionId = votacionId;
            ProyectoId = proyectoId;
            PuntuacionBase = puntuacionBase;
            Anonimo = anonimo;
            HashAnonimo = hashAnonimo;
            Comentario = comentario;
            Fecha = DateTime.UtcNow;
        }

        public abstract double CalcularPuntuacionNormalizada();
        public abstract string RolVotante();

        // Métodos abstractos para manejar al emisor correctamente
        public abstract void AsignarEmisorId(int id);

        // Metodo para la Auditoría de votos
        public abstract string ObtenerIdentificadorAuditoria();
    }
}