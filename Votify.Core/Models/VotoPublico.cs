namespace Votify.Core.Models
{
    public class VotoPublico : Voto
    {
        // Clave foránea: El voto público puede estar vinculado a un votante específico, pero no es obligatorio
        public int? VotanteId { get; set; }
        public virtual Votante? Votante { get; set; }

        protected VotoPublico() { }

        public VotoPublico(int votacionId, int proyectoId, double puntuacionBase, bool anonimo = false, string? hashAnonimo = null, string? comentario = null)
            : base(votacionId, proyectoId, puntuacionBase, anonimo, hashAnonimo, comentario) { }

        public override double CalcularPuntuacionNormalizada() => PuntuacionBase * 0.85;
        public override string RolVotante() => "PUBLIC";

        // Implementación de los métodos abstractos
        public override void AsignarEmisorId(int id) => VotanteId = id;

        public override string ObtenerIdentificadorAuditoria()
        {
            if (Anonimo || !string.IsNullOrWhiteSpace(HashAnonimo))
                return HashAnonimo ?? "HASH-ERROR";

            return Votante != null ? Votante.Email : $"Votante ID: {VotanteId}";
        }
    }
}