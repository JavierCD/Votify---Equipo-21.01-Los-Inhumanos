using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models;
using Votify.Core.Interfaces;
using Votify.Persistence.Context;

namespace Votify.Persistence.Repositories
{
    public class VotoExpertoRepository:IVotoExpertoRepository
    {
        private readonly VotifyContext _context;

        public VotoExpertoRepository(VotifyContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Proyecto>> ObtenerProyectosPorCategoriaAsync(int categoriaId)
        {
            return await _context.Proyectos
                .Where(p => p.Categorias.Any(c => c.Id == categoriaId) && p.Visible)
                .ToListAsync();
        }

        public async Task GuardarComentarioAsync(Voto voto)
        {
            //voto.VotanteId = null;
            _context.Votos.Add(voto);
        }

        public async Task<IEnumerable<Voto>> ObtenerComentariosPorCategoriaAsync(int categoriaId)
        {
            // Magia: OfType<VotoExperto>() filtra automáticamente los expertos
            return await _context.Votos.OfType<VotoExperto>()
                .Include(v => v.Proyecto)
                .Where(v => v.Votacion.CategoriaId == categoriaId && v.Comentario != null)
                .ToListAsync();
        }

        public async Task<bool> YaComentoPorProyectoAsync(int juezId, int proyectoId, int categoriaId)
        {
            // Usamos OfType y buscamos directamente por JuezId
            return await _context.Votos.OfType<VotoExperto>()
                .AnyAsync(v => v.JuezId == juezId
                            && v.ProyectoId == proyectoId
                            && v.Votacion.CategoriaId == categoriaId);
        }

        public async Task<IEnumerable<DetalleVoto>> ObtenerEvaluacionesPorProyectoYCriterioAsync(int proyectoId, int criterioId)
        {
            var emailsJueces = await _context.Miembros.OfType<Juez>()
                .Select(j => j.Email)
                .ToListAsync();

            return await _context.Set<DetalleVoto>()
                .Include(d => d.Voto)
                    .ThenInclude(v => (v as VotoPublico)!.Votante)
                .Where(d => d.ProyectoId == proyectoId
                          && d.CriterioId == criterioId
                          && d.Voto is VotoPublico
                          && ((VotoPublico)d.Voto).Votante != null
                          && emailsJueces.Contains(((VotoPublico)d.Voto).Votante!.Email))
                .OrderByDescending(d => d.Voto.Fecha)
                .ToListAsync();
        }

        public async Task<IEnumerable<Criterio>> ObtenerCriteriosPorProyectoAsync(int proyectoId)
        {
            
            return await _context.Votaciones
                .OfType<Multicriterio>()
                .Where(m => m.Categoria.Proyectos.Any(p => p.Id == proyectoId))
                .SelectMany(m => m.Criterios)
                .Distinct()
                .ToListAsync();
        }
   
        public async Task<IEnumerable<VotoExperto>> ObtenerComentariosJuezPorProyectoAsync(int proyectoId)
        {
            return await _context.Votos.OfType<VotoExperto>()
                .Include(v => v.Juez)
                .Where(v => v.ProyectoId == proyectoId
                          && v.Comentario != null
                          && v.Comentario != "")
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();
        }
        public async Task<Dictionary<string, string>> ObtenerMapaJuecesAsync()
        {
            return await _context.Miembros.OfType<Juez>()
                .ToDictionaryAsync(j => j.Email.ToLower(), j => j.Name);
        }
    }
}
