using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Services.Models;

namespace Votify.Services.Interfaces
{
    public interface IPopularService
    {
        Task<PopularResponse> CrearVotacionPopularAsync(CrearVotacionPopularRequest request);
        Task<VotacionPopularDisponibleResponse?> ObtenerProyectosParaVotarAsync(int votacionId, int votanteId);
    }
}
