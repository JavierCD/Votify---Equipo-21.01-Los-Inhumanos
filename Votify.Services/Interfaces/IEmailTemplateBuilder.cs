using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Services.Models.DTOs;

namespace Votify.Services.Interfaces
{
    public interface IEmailTemplateBuilder
    {
        string GenerarTablaResultadosHtml(string nombreCategoria, List<PosicionRankingDto> ranking);
    }
}
