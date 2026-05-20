using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Services.Models.Responses;

namespace Votify.Services.Interfaces
{
    public interface ISintesisIAService
    {
        Task<SintesisIAResponse?> ObtenerSintesisPorProyectoAsync(int proyectoId);
        Task<SintesisIAResponse> GenerarSintesisAsync(int proyectoId);
    }
}
