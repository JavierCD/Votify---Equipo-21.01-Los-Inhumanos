namespace Votify.Core.Models
{
    public class VotoExperto : Voto
    {
        // Clave foránea: El voto de experto debe estar vinculado a un juez específico
        public int? JuezId { get; set; }
        public virtual Juez? Juez { get; set; }

        protected VotoExperto() { }

        public VotoExperto(int votacionId, int proyectoId, double puntuacionBase, bool anonimo = false, string? hashAnonimo = null, string? comentario = null)
            : base(votacionId, proyectoId, puntuacionBase, anonimo, hashAnonimo, comentario) { }

        public override double CalcularPuntuacionNormalizada() => PuntuacionBase * 1.20;
        public override string RolVotante() => "EXPERT";

        // Implementación de los métodos abstractos
        public override void AsignarEmisorId(int id) => JuezId = id;

        public override string ObtenerIdentificadorAuditoria()
        {
            if (Anonimo || !string.IsNullOrWhiteSpace(HashAnonimo))
                return HashAnonimo ?? "HASH-ERROR";

            return Juez != null ? Juez.Name : $"Juez ID: {JuezId}";
        }
    }
}