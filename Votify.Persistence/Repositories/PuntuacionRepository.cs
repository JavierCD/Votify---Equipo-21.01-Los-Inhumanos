using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Persistence.Context;

namespace Votify.Persistence.Repositories
{
    public class PuntuacionRepository:IPuntuacionRepository
    {
        private readonly VotifyContext _context;
        public PuntuacionRepository(VotifyContext context)
        {
            _context = context;
        }
        public async Task<bool> CategoriaExisteAsync(int categoriaId)
        {
            return await _context.Categorias.AnyAsync(c => c.Id == categoriaId);
        }
        public async Task<Puntuacion> CrearAsync(Puntuacion puntuacion)
        {
            _context.Set<Puntuacion>().Add(puntuacion);
            return puntuacion;
        }

        public async Task<bool> YaExisteVotacionParaCategoriaAsync(int categoriaId)
        {
            return await _context.Set<Votacion>()
                .OfType<Puntuacion>()
                .AnyAsync(v => v.CategoriaId == categoriaId);
        }

        public async Task<Puntuacion?> ObtenerPorIdConCategoriaAsync(int votacionId)
        {
            return await _context.Set<Votacion>()
                .OfType<Puntuacion>()
                .Include(v => v.Categoria)
                .FirstOrDefaultAsync(v => v.Id == votacionId);
        }
    }
}
