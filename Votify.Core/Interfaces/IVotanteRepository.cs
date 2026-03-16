using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models;

namespace Votify.Core.Interfaces
{
    public interface IVotanteRepository
    {
        public interface IVotanteRepository
        {
            void Insertar(Votante modelo);
            Votante ObtenerPorId(int id);
            // ... otros métodos que necesites
        }
    }
}
