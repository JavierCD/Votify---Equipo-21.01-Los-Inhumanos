using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core;
using Votify.Core.Enums;
using Votify.Core.Factories;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Models;

namespace Votify.Services.Implementations
{
    public class EventoService : IEventoService
    {
        private readonly IEventoRepository _repository;
        private readonly IGenericRepository<Juez> _juezRepository;

        public EventoService(IEventoRepository repository, IGenericRepository<Juez> juezRepository)
        {
            _repository = repository;
            _juezRepository = juezRepository;
        }

        public async Task ActualizarAsync(EditarEventoRequest eventoMod)
        {
            var eventoExistente = await _repository.GetByIdAsync(eventoMod.Id);
            if (eventoExistente == null) 
            {
                throw new KeyNotFoundException($"No se encontró el evento con ID {eventoMod.Id}");
            }
            
            eventoExistente.ActualizarDatosGenerales(
                eventoMod.Name, 
                eventoMod.FechaInicio, 
                eventoMod.FechaFin, 
                eventoMod.Description);

            await _repository.UpdateAsync(eventoExistente);

        }

        public async Task<Evento?> ObtenerEventoConDetallesAsync(int id)
        {
            return await _repository.ObtenerEventoConDetallesAsync(id);
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

        public async Task<Evento?> ObtenerEventoPorCodigoAsync(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo)) return null;

            return await _repository.GetWithIncludesAsync(
                e => e.CodigoAcceso.ToUpper() == codigo.ToUpper(),
                e => e.CategoriasEvento
            );
        }
        public async Task<IEnumerable<Evento>> ObtenerEventosPorJuezAsync(int juezId)
        {
            if (juezId <= 0)
                throw new ArgumentException("El ID del juez no es válido.");

            return await _repository.ObtenerEventosPorJuezAsync(juezId);
        }

        public async Task AsignarJuezAEventoAsync(int juezId, int eventoId)
        {
            // 1. Usamos tu GenericRepository para traer el evento INCLUYENDO su jurado actual
            var evento = await _repository.GetWithIncludesAsync(e => e.Id == eventoId, e => e.Jurado);
            var juez = await _juezRepository.GetByIdAsync(juezId);

            if (evento == null || juez == null)
            {
                throw new ArgumentException("Evento o Juez no encontrados.");
            }

            // 2. Evitamos duplicados en la tabla intermedia EventosJurado
            if (evento.Jurado.Any(j => j.Id == juezId))
            {
                throw new InvalidOperationException("El juez ya está asignado a este evento.");
            }

            // 3. Lo añadimos a la lista y actualizamos
            evento.Jurado.Add(juez);
            await _repository.UpdateAsync(evento);
        }

        public async Task DesasignarJuezAEventoAsync(int juezId, int eventoId)
        {
            // Traemos el evento con su lista de jurado usando tu repositorio genérico
            var evento = await _repository.GetWithIncludesAsync(e => e.Id == eventoId, e => e.Jurado);

            if (evento == null)
                throw new ArgumentException("Evento no encontrado.");

            // Buscamos si el juez realmente está en la lista de este evento
            var juezARemover = evento.Jurado.FirstOrDefault(j => j.Id == juezId);

            if (juezARemover == null)
                throw new InvalidOperationException("El juez no está asignado a este evento.");

            // Lo quitamos de la lista y actualizamos
            evento.Jurado.Remove(juezARemover);
            await _repository.UpdateAsync(evento);
        }

        public async Task CrearEventoDesdeFormularioAsync(CrearEventoRequest modelo)
        {
            // 1. Resolvemos dependencias internas (como el organizador)
            int organizadorRealId = await ObtenerOrganizadorMockIdAsync();

            // 2. Usamos el Factory Pattern en la capa de Aplicación/Dominio, NO en la UI
            EventoCreator creator = modelo.TipoEvento switch
            {
                "Hackathon" => new HackathonEventCreator(),
                "Feria de Innovación" => new InnovationFairEventCreator(),
                "E-Sports" => new ESportsEventCreator(),
                _ => new HackathonEventCreator() // Fallback
            };

            // 3. Instanciamos la entidad de Dominio
            Evento nuevoEvento = creator.CrearEvento(
                modelo.Nombre,
                modelo.FechaInicio.ToUniversalTime(),
                modelo.FechaFin.ToUniversalTime(),
                organizadorRealId,
                modelo.Descripcion
            );

            // 4. Delegamos al Aggregate Root la creación de sus hijos (Categorías)
            foreach (var cat in modelo.Categorias)
            {
                nuevoEvento.AgregarCategoria(cat.Nombre, cat.Descripcion);
            }

            // 5. Persistimos a través de Infraestructura
            await _repository.AddAsync(nuevoEvento);
            // (Asegúrate de que AddAsync llame a SaveChangesAsync internamente, o llámalo aquí)
        }
    }
}
