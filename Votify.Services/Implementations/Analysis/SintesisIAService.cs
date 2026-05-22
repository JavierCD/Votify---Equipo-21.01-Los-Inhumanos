using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Interfaces;
using Votify.Services.Models.Responses;

namespace Votify.Services.Implementations
{
    public class SintesisIAService : ISintesisIAService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIAProvider _iaProvider;
        private readonly IVotoExpertoServices _votoExpertoService;

        public SintesisIAService(
            IUnitOfWork unitOfWork,
            IIAProvider iaProvider,
            IVotoExpertoServices votoExpertoService)
        {
            _unitOfWork = unitOfWork;
            _iaProvider = iaProvider;
            _votoExpertoService = votoExpertoService;
        }

        public async Task<SintesisIAResponse?> ObtenerSintesisPorProyectoAsync(int proyectoId)
        {
            var todas = await _unitOfWork.SintesisIA.GetAllAsync();
            var sintesis = todas.FirstOrDefault(s => s.ProyectoId == proyectoId);

            if (sintesis == null) return null;

            return new SintesisIAResponse
            {
                PuntosFuertes = sintesis.PuntosFuertes,
                AreasMejora = sintesis.AreasMejora,
                ConsensoGeneral = sintesis.ConsensoGeneral,
                FechaGeneracion = sintesis.FechaGeneracion,
                Existe = true
            };
        }

        public async Task<SintesisIAResponse> GenerarSintesisAsync(int proyectoId)
        {
            // 1. Obtener todos los comentarios del jurado para este proyecto
            var comentariosJuez = await _votoExpertoService.ObtenerComentariosJuezPorProyectoAsync(proyectoId);

            var comentariosConTexto = comentariosJuez
                .Where(c => !string.IsNullOrWhiteSpace(c.Comentario))
                .ToList();

            if (comentariosConTexto.Count < 2)
                throw new InvalidOperationException("Se necesitan al menos 2 comentarios del jurado para generar una síntesis.");

            // 2. Construir el prompt anonimizado (PA3)
            var prompt = ConstruirPrompt(comentariosConTexto);

            // 3. Llamar a la IA
            var respuestaIA = await _iaProvider.AnalizarAsync(prompt);
            Console.WriteLine($"========== RESPUESTA IA CRUDA ==========\n{respuestaIA}\n========================================");

            // 4. Parsear la respuesta
            var resultado = ParsearRespuesta(respuestaIA);

            // 5. Guardar en BD (borrar la anterior si existe)
            var todas = await _unitOfWork.SintesisIA.GetAllAsync();
            var existente = todas.FirstOrDefault(s => s.ProyectoId == proyectoId);
            if (existente != null)
            {
                await _unitOfWork.SintesisIA.DeleteAsync(existente.Id);
            }

            var nuevaSintesis = new SintesisIA
            {
                ProyectoId = proyectoId,
                PuntosFuertes = resultado.PuntosFuertes,
                AreasMejora = resultado.AreasMejora,
                ConsensoGeneral = resultado.ConsensoGeneral,
                FechaGeneracion = DateTime.UtcNow
            };

            await _unitOfWork.SintesisIA.AddAsync(nuevaSintesis);
            await _unitOfWork.SaveChangesAsync();

            resultado.FechaGeneracion = nuevaSintesis.FechaGeneracion;
            resultado.Existe = true;
            return resultado;
        }

        private string ConstruirPrompt(List<EvaluacionJuezResponse> comentarios)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Eres un asistente experto en análisis de evaluaciones de proyectos.");
            sb.AppendLine("A continuación te presento comentarios ANÓNIMOS de distintos evaluadores sobre un proyecto.");
            sb.AppendLine("IMPORTANTE: No hagas referencia a ningún evaluador individual, ni uses frases como 'el evaluador 1 dijo' o 'un juez mencionó'. Sintetiza las ideas de forma colectiva.");
            sb.AppendLine();
            sb.AppendLine("Comentarios de los evaluadores:");
            sb.AppendLine();

            int i = 1;
            foreach (var c in comentarios)
            {
                sb.AppendLine($"- Evaluación {i} (Puntuación: {c.Puntuacion:F1}): {c.Comentario}");
                i++;
            }

            sb.AppendLine();
            sb.AppendLine("Genera un resumen estructurado en formato JSON con exactamente estos tres campos:");
            sb.AppendLine("{");
            sb.AppendLine("  \"puntosFuertes\": \"texto resumiendo los puntos fuertes destacados\",");
            sb.AppendLine("  \"areasMejora\": \"texto resumiendo las principales áreas de mejora\",");
            sb.AppendLine("  \"consensoGeneral\": \"texto con la opinión general del conjunto de evaluadores\"");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("Responde SOLO con el JSON, sin texto adicional, sin markdown, sin backticks.");

            return sb.ToString();
        }

        private SintesisIAResponse ParsearRespuesta(string respuestaIA)
        {
            try
            {
                // Limpiar backticks y texto extra
                var limpio = respuestaIA
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();

                // Extraer solo el bloque JSON si hay texto antes o después
                int inicioJson = limpio.IndexOf('{');
                int finJson = limpio.LastIndexOf('}');

                if (inicioJson >= 0)
                {
                    finJson = limpio.LastIndexOf('}');
                    if (finJson > inicioJson)
                    {
                        limpio = limpio.Substring(inicioJson, finJson - inicioJson + 1);
                    }
                    else
                    {
                        // Ollama a veces no cierra el JSON — lo cerramos nosotros
                        limpio = limpio.Substring(inicioJson) + "}";
                    }
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // Parsear como diccionario genérico para capturar cualquier formato de claves
                // Fix: si el último valor no cierra comillas, cerrarlas
                if (limpio.EndsWith("\"}") == false && limpio.EndsWith("}"))
                {
                    // Verificar si falta cerrar comillas antes del }
                    var sinUltimo = limpio.TrimEnd('}').TrimEnd();
                    if (!sinUltimo.EndsWith("\""))
                    {
                        limpio = sinUltimo + "\"}";
                    }
                }
                var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(limpio, options);

                if (dict != null)
                {
                    return new SintesisIAResponse
                    {
                        PuntosFuertes = dict.GetValueOrDefault("puntosFuertes")
                            ?? dict.GetValueOrDefault("PuntosFuertes")
                            ?? dict.GetValueOrDefault("puntos_fuertes")
                            ?? "No se pudo extraer.",
                        AreasMejora = dict.GetValueOrDefault("areasMejora")
                            ?? dict.GetValueOrDefault("AreasMejora")
                            ?? dict.GetValueOrDefault("areas_mejora")
                            ?? "No se pudo extraer.",
                        ConsensoGeneral = dict.GetValueOrDefault("consensoGeneral")
                            ?? dict.GetValueOrDefault("ConsensoGeneral")
                            ?? dict.GetValueOrDefault("consenso_general")
                            ?? "No se pudo extraer."
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SintesisIA] Error parseando respuesta: {ex.Message}");
                Console.WriteLine($"[SintesisIA] Respuesta cruda: {respuestaIA}");
            }

            return new SintesisIAResponse
            {
                PuntosFuertes = "La IA no pudo estructurar la respuesta correctamente.",
                AreasMejora = "Intenta regenerar la síntesis.",
                ConsensoGeneral = respuestaIA
            };
        }

        private class SintesisJsonDto
        {
            [System.Text.Json.Serialization.JsonPropertyName("puntosFuertes")]
            public string PuntosFuertes { get; set; } = string.Empty;

            [System.Text.Json.Serialization.JsonPropertyName("areasMejora")]
            public string AreasMejora { get; set; } = string.Empty;

            [System.Text.Json.Serialization.JsonPropertyName("consensoGeneral")]
            public string ConsensoGeneral { get; set; } = string.Empty;
        }

    }
}
