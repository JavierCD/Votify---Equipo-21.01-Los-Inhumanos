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
           new PlantillaBaremoDto
            {
                Id = "hackathon-gen",
                Titulo = "Hackathon General",
                Descripcion = "Criterios balanceados para competiciones de desarrollo de software.",
                Criterios = new List<CriterioDto>
                {
                    new() { Nombre = "Innovación y Originalidad", Peso = 30 },
                    new() { Nombre = "Viabilidad Técnica", Peso = 30 },
                    new() { Nombre = "Diseño / UX", Peso = 20 },
                    new() { Nombre = "Presentación / Pitch", Peso = 20 }
                }
            },
            new PlantillaBaremoDto
            {
                Id = "pitch-eval",
                Titulo = "Evaluación de Pitch",
                Descripcion = "Enfocado en startups y rondas de inversión.",
                Criterios = new List<CriterioDto>
                {
                    new() { Nombre = "Modelo de Negocio", Peso = 40 },
                    new() { Nombre = "Problema y Solución", Peso = 30 },
                    new() { Nombre = "Equipo", Peso = 30 }
                }
            }

            // Añadir más plantillas según sea necesario...
        };

            return Task.FromResult(plantillas.AsEnumerable());
        }
    }
}
