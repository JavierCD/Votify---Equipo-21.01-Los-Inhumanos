using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Votify.Services.Interfaces;
using Votify.Services.Models.Responses;

namespace Votify.Services.Implementations
{
    public class PlantillaBaremoService : IPlantillaBaremoService
    {
        // 2. Cambiamos la firma para que devuelva el Response
        public Task<IEnumerable<PlantillaBaremoResponse>> ObtenerPlantillasPredefinidasAsync()
        {
            var plantillas = new List<PlantillaBaremoResponse>
            {
               new PlantillaBaremoResponse
                {
                    Id = "hackathon-gen",
                    Titulo = "Hackathon General",
                    Descripcion = "Criterios balanceados para competiciones de desarrollo de software.",
                    // 3. La lista interna ahora es de CriterioResponse
                    Criterios = new List<CriterioResponse>
                    {
                        new() { Nombre = "Innovación y Originalidad", Peso = 30 },
                        new() { Nombre = "Viabilidad Técnica", Peso = 30 },
                        new() { Nombre = "Diseño / UX", Peso = 20 },
                        new() { Nombre = "Presentación / Pitch", Peso = 20 }
                    }
                },
                new PlantillaBaremoResponse
                {
                    Id = "pitch-eval",
                    Titulo = "Evaluación de Pitch",
                    Descripcion = "Enfocado en startups y rondas de inversión.",
                    Criterios = new List<CriterioResponse>
                    {
                        new() { Nombre = "Modelo de Negocio", Peso = 40 },
                        new() { Nombre = "Problema y Solución", Peso = 30 },
                        new() { Nombre = "Equipo", Peso = 30 }
                    }
                }
            };

            return Task.FromResult(plantillas.AsEnumerable());
        }
    }
}