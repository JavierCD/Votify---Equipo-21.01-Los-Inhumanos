using Votify.Core.Models;

namespace Votify.Core.Factories
{
    public class VotoPublicoCreator : VotoCreator
    {
        public override Voto CrearVoto(int votacionId, int proyectoId, double puntuacionBase, bool anonimo = false, string? hashAnonimo = null,string? comentario=null)
        {
            return new VotoPublico(votacionId, proyectoId, puntuacionBase, anonimo, hashAnonimo,comentario);
        }
    }
}
