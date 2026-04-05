using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Core.Enums;

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
            
            eventoExistente.Id = evento.Id;
            eventoExistente.Description = evento.Description;

            if (evento.FechaFin <= evento.FechaInicio)
            {
                throw new ArgumentException("La fecha de fin debe ser posterior a la fecha de inicio");
            }
            
            eventoExistente.FechaFin = evento.FechaFin;
            eventoExistente.FechaInicio = evento.FechaInicio;

            await _repository.UpdateAsync(eventoExistente);

        }

        public async Task<Evento?> ObtenerEventoConDetallesAsync(int id)
        {
            return await _repository.GetWithIncludesAsync(
                e => e.Id == id,
                e => e.CategoriasEvento,
                e => e.Participantes,
                e => e.Organizador);
        }

        public async Task<Evento> CrearAsync(Evento evento)
        {
            if (evento.FechaFin <= evento.FechaInicio)
            {
                throw new ArgumentException("La fecha de fin debe ser posterior a la fecha de inicio");
            }


            return await _repository.AddAsync(evento);
        }

        public async Task EliminarAsync(int id)
        {
            var evento = await _repository.GetByIdAsync(id);
            if (evento == null)
            {
                throw new KeyNotFoundException($"No se encontró el evento con ID {id}");
            }
            if (evento.Estado == EstadoEvento.Cerrado || evento.Estado == EstadoEvento.Activo)
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
        public async Task<IEnumerable<Evento>> ObtenerEventosPorOrganizadorAsync(int organizadorId)
        {
            // Traemos todos los eventos incluyendo sus categorías
            var eventos = await _repository.GetAllWithIncludesAsync(e => e.CategoriasEvento);

            // Filtramos por el organizador y devolvemos
            return eventos.Where(e => e.OrganizadorId == organizadorId);
        }

        public async Task<int> ObtenerOrganizadorMockIdAsync()
        {
            // Buscamos cualquier evento que ya exista (el del Seeder) y le "robamos" el ID del organizador.
            // Es un hack inofensivo que no toca la tabla de Usuarios directamente.
            var eventoDemo = await _repository.GetAllAsync();
            return eventoDemo.FirstOrDefault()?.OrganizadorId ?? 1; // Si no hay, devolvemos 1 por si acaso
        }
    }
}
