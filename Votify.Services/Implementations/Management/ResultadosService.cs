using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Implementations.Strategies;
using Votify.Services.Interfaces;
using Votify.Services.Models.Requests;
using Votify.Services.Models.Responses;

namespace Votify.Services.Implementations
{
    public class ResultadosService : IResultadosService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateBuilder _templateBuilder;
        private readonly RankingStrategyFactory _rankingStrategyFactory;

        public ResultadosService(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IEmailTemplateBuilder templateBuilder,
            RankingStrategyFactory rankingStrategyFactory)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _templateBuilder = templateBuilder;
            _rankingStrategyFactory = rankingStrategyFactory;
        }

        public async Task<int> CompartirClasificacionAsync(int categoriaId)
        {
            var categoria = await _unitOfWork.CategoriaRepository.ObtenerCategoriaConVotacionYVotosAsync(categoriaId);

            if (categoria == null || categoria.Votacion == null)
                throw new Exception("Categoría o Votación no encontrada.");

            categoria.Votacion.CompartirResultados();
            await _unitOfWork.CategoriaRepository.UpdateAsync(categoria);
            await _unitOfWork.SaveChangesAsync();

            var premios = categoria.Premios.OrderBy(p => p.Posicion).ToList();

            List<PosicionRankingResponse> ranking;

            var intervenidos = (await _unitOfWork.ResultadosIntervenidos.GetAllWithIncludesAsync(r => r.Proyecto))
                .Where(r => r.VotacionId == categoria.Votacion.Id)
                .OrderBy(r => r.Posicion)
                .ToList();

            if (intervenidos.Any())
            {
                ranking = intervenidos.Select(ri => new PosicionRankingResponse
                {
                    Posicion = ri.Posicion,
                    NombreProyecto = ri.Proyecto.Name,
                    PuntosTotales = ri.PuntajeOriginal,
                    FechaInscripcion = ri.Proyecto.FechaRegistro,
                    PremioGanado = premios.FirstOrDefault(p => p.Posicion == ri.Posicion)?.Name ?? "Sin premio"
                }).ToList();
            }
            else
            {
                ranking = CalcularRankingConEmpates(categoria);

                for (int i = 0; i < ranking.Count; i++)
                {
                    ranking[i].Posicion = i + 1;
                }
            }

            var correosVotantes = categoria.Votacion.Votos
                .OfType<VotoPublico>()
                .Where(v => v.Votante != null && !string.IsNullOrWhiteSpace(v.Votante.Email))
                .Select(v => v.Votante!.Email)
                .Distinct()
                .ToList();

            if (correosVotantes.Any())
            {
                string asunto = $"🏆 Resultados Finales: {categoria.Name}";
                string cuerpoHtml = _templateBuilder.GenerarTablaResultadosHtml(categoria.Name, ranking);

                foreach (var email in correosVotantes)
                {
                    await _emailService.EnviarCorreoAsync(email, asunto, cuerpoHtml);
                }
            }

            return correosVotantes.Count;
        }

        private List<PosicionRankingResponse> CalcularRankingConEmpates(Categoria categoria)
        {
            var premios = categoria.Premios.OrderBy(p => p.Posicion).ToList();
            var estrategia = _rankingStrategyFactory.GetStrategy(categoria.Votacion);

            var proyectosPuntuados = estrategia.CalcularRanking(categoria);

            int posicionActual = 1;
            int contadorSaltos = 1;
            double puntosAnterior = double.MaxValue;

            foreach (var proyecto in proyectosPuntuados)
            {
                if (proyecto.PuntosTotales < puntosAnterior)
                {
                    posicionActual = contadorSaltos;
                }
                else if (proyecto.PuntosTotales == puntosAnterior)
                {
                    var premioActual = premios.FirstOrDefault(p => p.Posicion == posicionActual);
                    if (premioActual != null && !premioActual.PermiteEmpate)
                    {
                        posicionActual = contadorSaltos;
                    }
                }

                proyecto.Posicion = posicionActual;
                var premioGanado = premios.FirstOrDefault(p => p.Posicion == posicionActual);
                proyecto.PremioGanado = premioGanado != null ? premioGanado.Name : "Sin premio";

                puntosAnterior = proyecto.PuntosTotales;
                contadorSaltos++;
            }

            return proyectosPuntuados;
        }

        public async Task<List<ResultadoIntervenidoResponse>> ObtenerResultadosPorEventoAsync(int eventoId)
        {
            var evento = await _unitOfWork.EventoRepository.ObtenerEventoConDetallesAsync(eventoId);
            if (evento == null) throw new Exception("Evento no encontrado.");

            var resultado = new List<ResultadoIntervenidoResponse>();

            foreach (var categoria in evento.CategoriasEvento)
            {
                if (categoria.Votacion == null) continue;

                // Solo mostramos categorías con votación cerrada
                var votacion = categoria.Votacion;
                bool permiteCalculo = votacion.Estado == "Cerrada"
                                   || votacion.Estado == "CerradaManual"
                                   || votacion.EstaCerrada
                                   || votacion.ResultadosPublicados
                                   || votacion.MostrarRanking; // <-- NUEVO FLAG

                // Verificar si hay intervención guardada
                var intervenidos = (await _unitOfWork.ResultadosIntervenidos.GetAllAsync())
                    .Where(r => r.VotacionId == votacion.Id)
                    .OrderBy(r => r.Posicion)
                    .ToList();

                var dto = new ResultadoIntervenidoResponse
                {
                    CategoriaId = categoria.Id,
                    CategoriaNombre = categoria.Name,
                    VotacionId = votacion.Id,
                    EstadoVotacion = votacion.Estado,
                    TieneIntervencion = intervenidos.Any()
                };

                if (intervenidos.Any())
                {
                    // Mostrar el ranking intervenido guardado
                    foreach (var ri in intervenidos)
                    {
                        var proyecto = categoria.Proyectos.FirstOrDefault(p => p.Id == ri.ProyectoId);
                        dto.Proyectos.Add(new ProyectoResultadoResponse
                        {
                            ProyectoId = ri.ProyectoId,
                            NombreProyecto = proyecto?.Name ?? "Proyecto eliminado",
                            NombreEquipo = proyecto?.Participante?.Name ?? "Desconocido",
                            Puntaje = ri.PuntajeOriginal,
                            Posicion = ri.Posicion
                        });
                    }
                }
                else
                {
                    // Calcular ranking automático (reutilizamos la lógica existente)
                    var categoriaCompleta = await _unitOfWork.CategoriaRepository
                        .ObtenerCategoriaConVotacionYVotosAsync(categoria.Id);

                    if (categoriaCompleta != null)
                    {
                        var ranking = CalcularRankingConEmpates(categoriaCompleta);
                        int pos = 1;
                        foreach (var r in ranking)
                        {
                            var proyecto = categoriaCompleta.Proyectos
                                .FirstOrDefault(p => p.Name == r.NombreProyecto);
                            dto.Proyectos.Add(new ProyectoResultadoResponse
                            {
                                ProyectoId = proyecto?.Id ?? 0,
                                NombreProyecto = r.NombreProyecto,
                                NombreEquipo = proyecto?.Participante?.Name ?? "Desconocido",
                                Puntaje = r.PuntosTotales,
                                Posicion = pos++
                            });
                        }
                    }
                }

                // Proyectos de la categoría que NO están en el ranking actual (para "Agregar")
                var idsEnRanking = dto.Proyectos.Select(p => p.ProyectoId).ToHashSet();
                dto.ProyectosDisponibles = categoria.Proyectos
                    .Where(p => !idsEnRanking.Contains(p.Id))
                    .Select(p => new ProyectoResultadoResponse
                    {
                        ProyectoId = p.Id,
                        NombreProyecto = p.Name,
                        NombreEquipo = p.Participante?.Name ?? "Desconocido",
                        Puntaje = 0,
                        Posicion = 0
                    })
                    .ToList();

                resultado.Add(dto);
            }

            return resultado;
        }
        public async Task GuardarResultadosIntervenidosAsync(int votacionId, List<GuardarResultadoRequest> resultados)
        {
            // Borrar intervenciones previas de esta votación
            var existentes = (await _unitOfWork.ResultadosIntervenidos.GetAllAsync())
                .Where(r => r.VotacionId == votacionId)
                .ToList();

            foreach (var existente in existentes)
            {
                await _unitOfWork.ResultadosIntervenidos.DeleteAsync(existente.Id);
            }

            // Insertar los nuevos
            foreach (var item in resultados)
            {
                await _unitOfWork.ResultadosIntervenidos.AddAsync(new ResultadoIntervenido
                {
                    VotacionId = votacionId,
                    ProyectoId = item.ProyectoId,
                    Posicion = item.Posicion,
                    PuntajeOriginal = item.PuntajeOriginal,
                    FechaIntervencion = DateTime.UtcNow
                });
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
