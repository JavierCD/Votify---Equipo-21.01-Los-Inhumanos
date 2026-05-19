using System.Text.Json;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Interfaces;

namespace Votify.Services.Implementations.Analysis
{
    public class AnalisisMejoraService : IAnalisisMejoraService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIAProvider _iaProvider;

        public AnalisisMejoraService(IUnitOfWork unitOfWork, IIAProvider iaProvider)
        {
            _unitOfWork = unitOfWork;
            _iaProvider = iaProvider;
        }

        public async Task<HojaRutaMejora> GenerarHojaRutaAsync(int proyectoId)
        {
            if (proyectoId <= 0)
                throw new ArgumentException("ProyectoId no válido.");

            var comentarios = await _unitOfWork.VotoExpertoRepository.ObtenerComentariosPorProyectoAsync(proyectoId);
            var comentariosLista = comentarios.ToList();

            var nombreProyecto = comentariosLista.FirstOrDefault()?.Proyecto?.Name ?? "Desconocido";

            if (!comentariosLista.Any())
            {
                return new HojaRutaMejora
                {
                    ProyectoId = proyectoId,
                    ProyectoNombre = nombreProyecto,
                    TotalComentariosAnalizados = 0,
                    Sugerencias = new List<SugerenciaMejora>()
                };
            }

            var textoComentarios = string.Join("\n",
                comentariosLista.Select((v, i) =>
                {
                    var juez = v.Juez;
                    return $"Comentario {i + 1} (Juez: {juez?.Name ?? "Anónimo"}): {v.Comentario}";
                }));

            var prompt = $@"Eres un mentor técnico experto en desarrollo de software.
Analiza los siguientes comentarios de jueces sobre el proyecto '{nombreProyecto}'
y genera una hoja de ruta con sugerencias de mejora concretas y accionables.

Comentarios de los jueces:
{textoComentarios}

Responde ÚNICAMENTE con un JSON válido en este formato exacto:
{{
    ""sugerencias"": [
        {{
            ""prioridad"": 1,
            ""categoria"": ""Código"",
            ""descripcion"": ""Descripción del problema"",
            ""accionRecomendada"": ""Qué hacer para mejorar""
        }}
    ]
}}

Máximo 5 sugerencias. Prioridad del 1 (más urgente) al 5.";

            var respuestaIA = await _iaProvider.AnalizarAsync(prompt);

            var sugerencias = ParsearRespuestaIA(respuestaIA);

            return new HojaRutaMejora
            {
                ProyectoId = proyectoId,
                ProyectoNombre = nombreProyecto,
                TotalComentariosAnalizados = comentariosLista.Count,
                FechaGeneracion = DateTime.UtcNow,
                Sugerencias = sugerencias
            };
        }

        private List<SugerenciaMejora> ParsearRespuestaIA(string respuesta)
        {
            var sugerencias = new List<SugerenciaMejora>();

            try
            {
                var doc = JsonDocument.Parse(respuesta);
                var array = doc.RootElement.GetProperty("sugerencias");

                foreach (var item in array.EnumerateArray())
                {
                    sugerencias.Add(new SugerenciaMejora
                    {
                        Prioridad = item.TryGetProperty("prioridad", out var p) ? p.GetInt32() : 99,
                        Categoria = item.TryGetProperty("categoria", out var c) ? c.GetString() ?? "" : "General",
                        Descripcion = item.TryGetProperty("descripcion", out var d) ? d.GetString() ?? "" : "",
                        AccionRecomendada = item.TryGetProperty("accionRecomendada", out var a) ? a.GetString() ?? "" : ""
                    });
                }
            }
            catch
            {
            }

            return sugerencias.OrderBy(s => s.Prioridad).ToList();
        }
    }
}
