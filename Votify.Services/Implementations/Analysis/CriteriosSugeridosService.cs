using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Services.Interfaces;
using Votify.Services.Models.Requests;

namespace Votify.Services.Implementations.Analysis
{
    public class CriteriosSugeridosService : ICriteriosSugeridosService
    {
        private readonly IIAProvider _iaProvider;

        public CriteriosSugeridosService(IIAProvider iaProvider)
        {
            _iaProvider = iaProvider;
        }

        public async Task<List<CriterioRequest>> SugerirCriteriosAsync(string nombreCategoria, string? descripcionCategoria)
        {
            var descripcionTexto = string.IsNullOrWhiteSpace(descripcionCategoria)
                ? ""
                : $"con la siguiente descripción: \"{descripcionCategoria}\"";

            var prompt = $@"Eres un experto en evaluación de proyectos y competiciones tecnológicas.
Para la categoría ""{nombreCategoria}"" {descripcionTexto},
sugiere exactamente 5 criterios de evaluación relevantes con pesos que sumen exactamente 100.

Responde ÚNICAMENTE con un JSON válido en este formato:
[{{""nombre"": ""Innovación"", ""peso"": 25}}, {{""nombre"": ""Impacto"", ""peso"": 20}}]

Los pesos deben ser números enteros y la suma total debe ser exactamente 100.";

            var respuestaIA = await _iaProvider.AnalizarAsync(prompt);

            return ParsearRespuestaIA(respuestaIA);
        }

        private List<CriterioRequest> ParsearRespuestaIA(string respuesta)
        {
            try
            {
                var doc = JsonDocument.Parse(respuesta);
                var criterios = new List<CriterioRequest>();
                int pesoTotal = 0;

                foreach (var item in doc.RootElement.EnumerateArray())
                {
                    var nombre = item.TryGetProperty("nombre", out var n) ? n.GetString() ?? "" : "";
                    var peso = item.TryGetProperty("peso", out var p) ? p.GetInt32() : 0;

                    if (string.IsNullOrWhiteSpace(nombre) || peso <= 0)
                        continue;

                    criterios.Add(new CriterioRequest
                    {
                        Nombre = nombre,
                        Peso = peso
                    });

                    pesoTotal += peso;
                }

                if (!criterios.Any())
                    throw new InvalidOperationException("La IA no devolvió criterios válidos.");

                if (pesoTotal != 100)
                {
                    var diferencia = 100 - pesoTotal;
                    criterios.Last().Peso += diferencia;
                }

                return criterios;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al procesar la respuesta de la IA: {ex.Message}");
            }
        }
    }
}
