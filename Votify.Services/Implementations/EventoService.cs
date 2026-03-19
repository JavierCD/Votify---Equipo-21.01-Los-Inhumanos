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

        public EventoService(IGenericRepository<Evento> repository)
        {
            _repository = repository;
        }

        public async Task ActualizarAsync(Evento evento)
        {
            var eventoExistente = await _repository.GetByIdAsync(evento.Id);
            if (eventoExistente == null) 
            {
                throw new KeyNotFoundException($"No se encontró el evento con ID {evento.Id}");
            }
            await _repository.UpdateAsync(evento);
        }

        public async Task<Evento> CrearAsync(Evento evento)
        {
            if (evento.FechaFin <= evento.FechaInicio)
            {
                throw new ArgumentException("La fecha de fin debe ser posterior a la fecha de inicio");
            }

            evento.Estado = "Borrador"; // Falta crear un enum con los estados de los eventos

            return await _repository.AddAsync(evento);
        }

        public async Task EliminarAsync(int id)
        {
            var evento = await _repository.GetByIdAsync(id);
            if (evento == null)
            {
                throw new KeyNotFoundException($"No se encontró el evento con ID {id}");
            }
            if (evento.Estado == "Cerrado" || evento.Estado == "Activo")
            {
                throw new InvalidOperationException("No se puede eliminar un evento que esta activo o que ya fue cerrado.");
            }
            await _repository.DeleteAsync(id);
        }

        public async Task<Evento?> ObtenerPorIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Evento>> ObtenerTodosAsync()
        {
            return await _repository.GetAllAsync();
        }
    }
}
