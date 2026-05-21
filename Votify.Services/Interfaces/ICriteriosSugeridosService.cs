using System.Collections.Generic;
using System.Threading.Tasks;
using Votify.Services.Models.Requests;

namespace Votify.Services.Interfaces
{
    public interface ICriteriosSugeridosService
    {
        Task<List<CriterioRequest>> SugerirCriteriosAsync(string nombreCategoria, string? descripcionCategoria);
    }
}
