using Votify.Core.Models;

namespace Votify.Core.Factories
{
    public class SustainabilityProjectCreator : ProyectoCreator
    {
        public override Proyecto CrearProyecto(string name, int participanteId, double criterioA = 0, double criterioB = 0)
        {
            return new SustainabilityProject(name, participanteId, criterioA, criterioB);
        }
    }
}