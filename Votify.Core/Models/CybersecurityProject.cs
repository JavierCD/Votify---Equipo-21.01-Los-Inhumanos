namespace Votify.Core.Models
{

    public class CybersecurityProject : Proyecto
    {
        protected CybersecurityProject() { }

        public CybersecurityProject(string name, int participanteId, double criterioA = 0, double criterioB = 0, string? desc = null)
            : base(name, participanteId, criterioA, criterioB, desc) { }


        public override double CalcularPuntuacion() => (CriterioA * 0.70) + (CriterioB * 0.30);
        public override string CategoriaEspecialidad() => "Cybersecurity";
    }
}