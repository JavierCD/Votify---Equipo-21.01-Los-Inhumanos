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
            var entity = new Popular
            {
                CategoriaId = popular.CategoriaId,
                FechaApertura = popular.FechaApertura,
                FechaCierre = popular.FechaCierre,
                Estado = popular.Estado,
                MaxSelection = popular.MaxSelection
            };

            _context.Votaciones.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
        public async Task<bool> YaExisteVotacionParaCategoriaAsync(int categoriaId)
        {
        return await _context.Votaciones.AnyAsync(v=> v.CategoriaId==categoriaId);
        }
    }
}