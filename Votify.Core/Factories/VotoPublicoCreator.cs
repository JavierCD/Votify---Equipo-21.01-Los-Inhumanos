using Votify.Core.Models;

namespace Votify.Core.Factories
{
    public class VotoPublicoCreator : VotoCreator
    {
        public override Voto CrearVoto(int votacionId, int proyectoId, double puntuacionBase, bool anonimo = false, string? hashAnonimo = null)
        {
            return new VotoPublico(votacionId, proyectoId, puntuacionBase, anonimo, hashAnonimo);
        }
    }
}
