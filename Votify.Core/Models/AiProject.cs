namespace Votify.Core.Models
{

    public class AiProject : Proyecto
    {
        protected AiProject() { }

        public AiProject(string name, int participanteId, double criterioA = 0, double criterioB = 0, string? desc = null)
            : base(name, participanteId, criterioA, criterioB, desc) { }

        public override double CalcularPuntuacion() => (CriterioA * 0.65) + (CriterioB * 0.35);
        public override string CategoriaEspecialidad() => "IA";
    }
}