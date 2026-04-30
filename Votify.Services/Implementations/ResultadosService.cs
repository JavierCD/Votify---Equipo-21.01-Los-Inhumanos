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
        private readonly IGenericRepository<Categoria> _categoriaRepo;
        private readonly IGenericRepository<Voto> _votoRepo;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateBuilder _templateBuilder; // Ver Paso 3

        public ResultadosService(
            IGenericRepository<Categoria> categoriaRepo,
            IGenericRepository<Voto> votoRepo,
            IEmailService emailService,
            IEmailTemplateBuilder templateBuilder)
        {
            _categoriaRepo = categoriaRepo;
            _votoRepo = votoRepo;
            _emailService = emailService;
            _templateBuilder = templateBuilder;
        }

        public async Task<int> CompartirClasificacionAsync(int categoriaId)
        {
            // 1. Obtener la categoría con su votación
            var categoria = await _categoriaRepo.GetAllAsync()
                .Include(c => c.Votacion)
                    .ThenInclude(v => v.Votos)
                        .ThenInclude(v => v.Proyecto)
                .Include(c => c.Votacion)
                    .ThenInclude(v => v.Votos)
                        .ThenInclude(v => v.Votante)
                .FirstOrDefaultAsync(c => c.Id == categoriaId);

            if (categoria == null || categoria.Votacion == null)
                throw new Exception("Categoría o Votación no encontrada.");

            // 2. Cambiamos el estado (Lógica de Dominio)
            categoria.Votacion.CompartirResultados();
            await _categoriaRepo.UpdateAsync(categoria);

            // 3. Calculamos el Ranking
            // NOTA: Asumo que sumarás la PuntuacionBase, si no, usa Count() como tenías
            var ranking = categoria.Votacion.Votos
                .GroupBy(v => v.Proyecto)
                .Select(g => new PosicionRankingDto
                {
                    NombreProyecto = g.Key?.Name ?? "Desconocido",
                    PuntosTotales = g.Sum(v => v.PuntuacionBase) // O g.Count() si es por cantidad
                })
                .OrderByDescending(x => x.PuntosTotales)
                .ToList();

            // Asignamos las posiciones (manejando posibles empates en un futuro si es necesario)
            for (int i = 0; i < ranking.Count; i++)
            {
                ranking[i].Posicion = i + 1;
            }

            // 4. Recopilamos los correos únicos
            var correosVotantes = categoria.Votacion.Votos
                .OfType<VotoPublico>() // Si aplica solo a voto público
                .Where(v => v.Votante != null && !string.IsNullOrWhiteSpace(v.Votante.Email))
                .Select(v => v.Votante!.Email)
                .Distinct()
                .ToList();

            // 5. Construir y Enviar Correos
            if (correosVotantes.Any())
            {
                string asunto = $"🏆 Resultados Finales: {categoria.Name}";
                string cuerpoHtml = _templateBuilder.GenerarTablaResultadosHtml(categoria.Name, ranking);

                foreach (var email in correosVotantes)
                {
                    await _emailService.EnviarCorreoAsync(email, asunto, cuerpoHtml);
                }
            }

            return correosVotantes.Count; // Retornamos a cuántos se envió
        }
    }
}
