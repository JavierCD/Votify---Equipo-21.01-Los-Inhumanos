using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Services.Models;

namespace Votify.Services.Interfaces
{
    public interface IVotoClientService
    {
        Task<bool> EmitirVotoMulticriterioAsync(EmitirVotoMulticriterioRequest request);
        Task<bool> EmitirVotoPuntuacionAsync(EmitirVotoPuntuacionRequest request);
        Task<bool> EmitirVotoPopularAsync(EmitirVotoPopularRequest request);
    }
}
