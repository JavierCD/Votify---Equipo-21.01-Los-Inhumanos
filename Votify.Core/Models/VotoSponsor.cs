namespace Votify.Core.Models
{
    public class VotoSponsor : Voto
    {
        // Clave foránea: El voto de sponsor debe estar vinculado a un sponsor específico
        public int? SponsorId { get; set; }
        // public virtual Sponsor? Sponsor { get; set; } // Descomenta esto si existe la clase Sponsor

        protected VotoSponsor() { }

        public VotoSponsor(int votacionId, int proyectoId, double puntuacionBase, bool anonimo = false, string? hashAnonimo = null, string? comentario = null)
            : base(votacionId, proyectoId, puntuacionBase, anonimo, hashAnonimo, comentario) { }

        public override double CalcularPuntuacionNormalizada() => PuntuacionBase * 0.95; // Penalización del 5%
        public override string RolVotante() => "SPONSOR";

        
        public override void AsignarEmisorId(int id) => SponsorId = id;

        public override string ObtenerIdentificadorAuditoria()
        {
            if (Anonimo || !string.IsNullOrWhiteSpace(HashAnonimo))
                return HashAnonimo ?? "HASH-ERROR";

            // Si tienes la clase Sponsor, pon Sponsor.Name
            return $"Sponsor ID: {SponsorId}";
        }
    }
}