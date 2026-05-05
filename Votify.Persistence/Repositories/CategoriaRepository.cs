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
    public class CategoriaRepository : GenericRepository<Categoria>, ICategoriaRepository
    {
        

        public CategoriaRepository(VotifyContext context) : base(context)
        {
        }

        public async Task<Categoria?> ObtenerCategoriaConVotacionYVotosAsync(int categoriaId)
        {
            return await _context.Categorias
                .Include(c => c.Votacion)
                    .ThenInclude(v => v.Votos)
                        .ThenInclude(v => v.Proyecto)
                .Include(c => c.Votacion)
                    .ThenInclude(v => v.Votos)
                        .ThenInclude(v => v.Detalles)
                .Include(c => c.Votacion)
                    .ThenInclude(v => ((Multicriterio)v).Criterios)
                .Include(c => c.Premios)
                .Include(c => c.Proyectos)
                .FirstOrDefaultAsync(c => c.Id == categoriaId);
        }



    }
}
