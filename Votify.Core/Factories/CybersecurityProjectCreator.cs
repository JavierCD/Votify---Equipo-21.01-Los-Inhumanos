using Votify.Core.Models;

namespace Votify.Core.Factories
{

    public class CybersecurityProjectCreator : ProyectoCreator 
    {
        public override Proyecto CrearProyecto(string name, int participanteId, double criterioA = 0, double criterioB = 0)
        {
            return new CybersecurityProject(name, participanteId, criterioA, criterioB);
        }
    }
}