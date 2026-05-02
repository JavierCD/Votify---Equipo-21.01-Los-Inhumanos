using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Interfaces;
using Votify.Services.Models.DTOs;

namespace Votify.Services.Implementations
{
    public class ResultadosService : IResultadosService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateBuilder _templateBuilder;

        public ResultadosService(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IEmailTemplateBuilder templateBuilder)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _templateBuilder = templateBuilder;
        }

        public async Task<int> CompartirClasificacionAsync(int categoriaId)
        {
            var categoria = await _unitOfWork.CategoriaRepository.ObtenerCategoriaConVotacionYVotosAsync(categoriaId);

            if (categoria == null || categoria.Votacion == null)
                throw new Exception("Categoría o Votación no encontrada.");

            categoria.Votacion.CompartirResultados();
            await _unitOfWork.CategoriaRepository.UpdateAsync(categoria);
            await _unitOfWork.SaveChangesAsync();



            List<PosicionRankingDto> ranking = CalcularRankingConEmpates(categoria);

            for (int i = 0; i < ranking.Count; i++)
            {
                ranking[i].Posicion = i + 1;
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

        /// <summary>
        /// Motor de cálculo de posiciones evaluando la regla "PermiteEmpate"
        /// </summary>
        private List<PosicionRankingDto> CalcularRankingConEmpates(Categoria categoria)
        {
            var premios = categoria.Premios.OrderBy(p => p.Posicion).ToList();

            // 1. Agrupar y sumar puntos. 
            // En caso de empate no permitido, desempataremos por quién se inscribió antes (FechaRegistro)
            var proyectosPuntuados = categoria.Votacion.Votos
                .Where(v => v.Proyecto != null)
                .GroupBy(v => v.Proyecto)
                .Select(g => new PosicionRankingDto
                {
                    NombreProyecto = g.Key!.Name,
                    PuntosTotales = g.Sum(v => v.PuntuacionBase),
                    FechaInscripcion = g.Key.FechaRegistro // Usado como criterio de desempate técnico
                })
                // Ordenamos por puntos (Descendente) y luego por FechaInscripcion (Ascendente) para casos de empate forzoso
                .OrderByDescending(x => x.PuntosTotales)
                .ThenBy(x => x.FechaInscripcion)
                .ToList();

            // 2. Lógica de Asignación de Posiciones
            int posicionActual = 1;
            int contadorSaltos = 1; // Para llevar la cuenta real de proyectos evaluados
            double puntosAnterior = double.MaxValue;

            foreach (var proyecto in proyectosPuntuados)
            {
                if (proyecto.PuntosTotales < puntosAnterior)
                {
                    // Si tiene menos puntos, baja a la posición del contador real
                    posicionActual = contadorSaltos;
                }
                else if (proyecto.PuntosTotales == puntosAnterior)
                {
                    // ¡HAY UN EMPATE MATEMÁTICO!
                    // Buscamos si el premio de la 'posicionActual' admite empate
                    var premioActual = premios.FirstOrDefault(p => p.Posicion == posicionActual);

                    if (premioActual != null && !premioActual.PermiteEmpate)
                    {
                        // REGLA DE NEGOCIO: No se permite empate. 
                        // Forzamos al proyecto a bajar a la siguiente posición.
                        // (Como ordenamos por FechaInscripcion, el que se inscribió tarde pierde el empate).
                        posicionActual = contadorSaltos;
                    }
                    // Si permite empate, 'posicionActual' no cambia y comparten el mismo puesto.
                }

                // Asignamos la posición calculada
                proyecto.Posicion = posicionActual;

                // 3. Asignar el nombre del premio ganado (si existe para esa posición)
                var premioGanado = premios.FirstOrDefault(p => p.Posicion == posicionActual);
                proyecto.PremioGanado = premioGanado != null ? premioGanado.Name : "Sin premio";

                // Actualizamos variables para la siguiente iteración
                puntosAnterior = proyecto.PuntosTotales;
                contadorSaltos++;
            }

            return proyectosPuntuados;
        }
    }
}
