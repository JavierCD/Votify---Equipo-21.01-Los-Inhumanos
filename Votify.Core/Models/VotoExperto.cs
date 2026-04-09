namespace Votify.Core.Models
{
    public class VotoExperto : Voto
    {
        protected VotoExperto() { }

        public VotoExperto(int votacionId, int proyectoId, double puntuacionBase, bool anonimo = false, string? hashAnonimo = null)
            : base(votacionId, proyectoId, puntuacionBase, anonimo, hashAnonimo) { }

        public override double CalcularPuntuacionNormalizada() => PuntuacionBase * 1.20; // 20% más de peso
        public override string RolVotante() => "EXPERT";
    }
}