namespace Votify.Core.Models
{
    public class VotoSponsor : Voto
    {
        protected VotoSponsor() { }

        public VotoSponsor(int votacionId, int proyectoId, double puntuacionBase, bool anonimo = false, string? hashAnonimo = null,string?comentario=null)
            : base(votacionId, proyectoId, puntuacionBase, anonimo, hashAnonimo,comentario) { }

        public override double CalcularPuntuacionNormalizada() => PuntuacionBase * 0.95; // Penalización del 5%
        public override string RolVotante() => "SPONSOR";
    }
}