using Microsoft.EntityFrameworkCore;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Persistence.Context;

namespace Votify.Persistence.Repositories
{
    public class PopularRepository : IPopularRepository
    {
        private readonly VotifyContext _context;

        public PopularRepository(VotifyContext context)
        {
            _context = context;
        }

        public async Task<bool> CategoriaExisteAsync(int categoriaId)
        {
            return await _context.Categorias.AnyAsync(c => c.Id == categoriaId);
        }
        

        public async Task<Popular> CrearAsync(Popular popular)
        {
            _context.Votaciones.Add(popular);
            await _context.SaveChangesAsync();

            return popular;
        }
        public async Task<bool> YaExisteVotacionParaCategoriaAsync(int categoriaId)
        {
            return await _context.Votaciones
             .OfType<Popular>()  
             .AnyAsync(v => v.CategoriaId == categoriaId);
        }
        public async Task<Popular?> ObtenerPorIdConCategoriaAsync(int id)
        {
            return await _context.Votaciones
                .OfType<Popular>()
                .Include(v => v.Categoria)
                  .ThenInclude(c=> c.Proyectos)
                .FirstOrDefaultAsync(v => v.Id == id);
        }
    }
}