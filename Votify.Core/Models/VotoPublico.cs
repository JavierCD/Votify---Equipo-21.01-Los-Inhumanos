namespace Votify.Core.Models
{
    public class VotoPublico : Voto
    {
        protected VotoPublico() { }

        public VotoPublico(int votacionId, int proyectoId, double puntuacionBase, bool anonimo = false, string? hashAnonimo = null)
            : base(votacionId, proyectoId, puntuacionBase, anonimo, hashAnonimo) { }

        public override double CalcularPuntuacionNormalizada() => PuntuacionBase * 0.85; // Penalización del 15%
        public override string RolVotante() => "PUBLIC";
    }
}