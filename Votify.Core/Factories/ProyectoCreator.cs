using Votify.Core.Models;

namespace Votify.Core.Factories
{
    public abstract class ProyectoCreator
    {
        public abstract Proyecto CrearProyecto(string name, int participanteId, double criterioA = 0, double criterioB = 0);
    }
}