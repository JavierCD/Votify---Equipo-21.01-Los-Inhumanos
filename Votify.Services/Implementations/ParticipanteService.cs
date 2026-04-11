using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Core.Models;

namespace Votify.Services.Implementations
{
    public class ParticipanteService : IParticipanteService
    {
        private readonly IGenericRepository<Participante> _repository;

        public ParticipanteService(IGenericRepository<Participante> repository)
        {
            _repository = repository;
        }

        public async Task<Participante?> ObtenerPorIdAsync(int id)
        {
            return await _repository.GetWithIncludesAsync(
                p => p.Id == id,
                p => p.Proyectos);
        }

        public async Task<IEnumerable<Participante>> ObtenerTodosAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task ActualizarFichaAsync(Participante participante)
        {
            var participanteExistente = await _repository.GetByIdAsync(participante.Id);
            if (participanteExistente == null)
            {
                throw new KeyNotFoundException($"No se encontró el participante con ID {participante.Id}");
            }

            // Usamos el nuevo método de dominio en lugar de cambiar la propiedad a pelo
            participanteExistente.ActualizarFicha(participante.Descripcion);
            
            participanteExistente.Name = participante.Name;
            
            await _repository.UpdateAsync(participanteExistente);
        }

        public async Task CambiarEstadoAsync(int id, string nuevoEstado)
        {
            var participante = await _repository.GetByIdAsync(id);
            if (participante == null)
            {
                throw new KeyNotFoundException($"No se encontró el participante con ID {id}");
            }

            // La entidad Participante se encarga de validar si el estado es correcto
            participante.EvaluarEstado(nuevoEstado);

            await _repository.UpdateAsync(participante);
        }

        public async Task CambiarVisibilidadAsync(int id, bool visible)
        {
            var participante = await _repository.GetByIdAsync(id);
            if (participante == null)
            {
                throw new KeyNotFoundException($"No se encontró el participante con ID {id}");
            }

            // Delegamos la acción a la entidad
            participante.CambiarVisibilidad(visible);

            await _repository.UpdateAsync(participante);
        }

        public async Task<Participante?> ObtenerDashboardAsync(int id)
        {
            // Usamos el GetWithIncludesAsync del GenericRepository.
            // El primer parámetro es el "WHERE" (p => p.Id == id).
            // El segundo parámetro es el "INCLUDE" (p => p.Proyecto).
            return await _repository.GetWithIncludesAsync(
                p => p.Id == id,
                p => p.Proyecto
            );
        }

    }
}
