using Votify.Core.Models;

namespace Votify.Core.Factories
{
    public abstract class VotoCreator
    {
        public abstract Voto CrearVoto(int votacionId, int proyectoId, double puntuacionBase, bool anonimo = false, string? hashAnonimo = null);
    }
}