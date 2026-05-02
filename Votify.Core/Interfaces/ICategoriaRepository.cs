using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models;

namespace Votify.Core.Interfaces
{
    public interface ICategoriaRepository : IGenericRepository<Categoria>
    {
        Task<Categoria?> ObtenerCategoriaConVotacionYVotosAsync(int categoriaId);
    }
}
