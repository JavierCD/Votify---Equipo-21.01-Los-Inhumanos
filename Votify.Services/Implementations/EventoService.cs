using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core;
using Votify.Core.Interfaces;
using Votify.Core.Models;

namespace Votify.Services.Implementations
{
    public class EventoService : IEventoService
    {
        private readonly IGenericRepository<Evento> _repository;

        public Task ActualizarAsync(Evento evento)
        {
            throw new NotImplementedException();
        }

        public Task<Evento> CrearAsync(Evento evento)
        {
            throw new NotImplementedException();
        }

        public Task EliminarAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Evento?> ObtenerPorIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Evento>> ObtenerTodosAsync()
        {
            throw new NotImplementedException();
        }
    }
}
