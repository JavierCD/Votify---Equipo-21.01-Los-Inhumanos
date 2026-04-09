namespace Votify.Core.Models
{
    public class SustainabilityProject : Proyecto
    {
        protected SustainabilityProject() { }

        public SustainabilityProject(string name, int participanteId, double criterioA = 0, double criterioB = 0)
            : base(name, participanteId, criterioA, criterioB) { }

        public override double CalcularPuntuacion() => (CriterioA * 0.55) + (CriterioB * 0.45);
        public override string CategoriaEspecialidad() => "Sostenibilidad";
    }
}