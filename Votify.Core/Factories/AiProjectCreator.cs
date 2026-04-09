using Votify.Core.Models;

namespace Votify.Core.Factories
{
    
    public class AiProjectCreator : ProyectoCreator
    {
        public override Proyecto CrearProyecto(string name, int participanteId, double criterioA = 0, double criterioB = 0)
        {
            return new AiProject(name, participanteId, criterioA, criterioB);
        }
    }
}