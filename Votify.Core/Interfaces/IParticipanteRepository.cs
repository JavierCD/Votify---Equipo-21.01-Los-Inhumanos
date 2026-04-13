using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models;

namespace Votify.Core.Interfaces
{
    public interface IParticipanteRepository : IGenericRepository<Participante>
    {
        Task<Participante?> ObtenerConDetallesDashboardAsync(int id);
    }
}
