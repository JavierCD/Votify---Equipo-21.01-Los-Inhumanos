using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Interfaces;
using Votify.Services.Models;
namespace Votify.Services.Interfaces
{
    public interface IVotoPopularService
    {
        Task<VotacionPopularDisponibleResponse> ObtenerVotacionPopularDisponibleAsync();
        Task EmitirVotoPopularAsync(EmitirVotoPopularRequest request);
    }
}
