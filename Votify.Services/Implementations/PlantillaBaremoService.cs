using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Services.Interfaces;
using Votify.Services.Models.DTOs;

namespace Votify.Services.Implementations
{
    public class PlantillaBaremoService :IPlantillaBaremoService
    {
        public Task<IEnumerable<PlantillaBaremoDto>> ObtenerPlantillasPredefinidasAsync()
        {
            var plantillas = new List<PlantillaBaremoDto>
        {
            new PlantillaBaremoDto(
                "hackathon-gen",
                "Hackathon General",
                "Ideal para competiciones de desarrollo de software con un enfoque equilibrado.",
                new List<CriterioPlantillaDto>
                {
                    new("Innovación y Originalidad", 30),
                    new("Viabilidad Técnica", 25),
                    new("Diseño y UX/UI", 20),
                    new("Presentación / Pitch", 25)
                }),

            new PlantillaBaremoDto(
                "pitch-eval",
                "Evaluación de Pitch",
                "Enfocado en startups y presentaciones de negocio.",
                new List<CriterioPlantillaDto>
                {
                    new("Problema y Solución", 25),
                    new("Modelo de Negocio", 25),
                    new("Tamaño de Mercado", 20),
                    new("Tracción y Roadmap", 15),
                    new("Equipo", 15)
                })

            // Añadir más plantillas según sea necesario...
        };

            return Task.FromResult(plantillas.AsEnumerable());
        }
    }
}
