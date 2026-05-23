using System.Collections.Generic;
using Votify.Core.Models;
using Votify.Services.Models.Responses;

namespace Votify.Services.Interfaces
{
    public interface IRankingStrategy
    {
        List<PosicionRankingResponse> CalcularRanking(Categoria categoria);
    }
}
