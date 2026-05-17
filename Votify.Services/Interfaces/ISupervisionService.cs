using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Services.Models.Responses;

namespace Votify.Services.Interfaces
{
    public interface ISupervisionService
    {
        Task<List<JuezSupervisionDto>> ObtenerEstadoJuecesAsync(int votacionId);
        Task EnviarRecordatorioAsync(int juezId, int votacionId, int categoriaId, string categoriaNombre, string eventoNombre);
       
    }
}
