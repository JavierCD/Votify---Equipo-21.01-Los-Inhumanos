using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Core.Models;

namespace Votify.Services.Implementations
{
    public class ParticipanteService : IParticipanteService
    {
        private readonly IParticipanteRepository _repository;

        public ParticipanteService(IParticipanteRepository repository)
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

            // 1. Actualizamos datos básicos
            participanteExistente.Name = participante.Name;
            participanteExistente.ActualizarFicha(participante.Descripcion);

            // 2. Actualizamos los nuevos campos del Perfil Público
            participanteExistente.InstitucionEducativa = participante.InstitucionEducativa;
            participanteExistente.Intereses = participante.Intereses;
            participanteExistente.ColorFondo = participante.ColorFondo;
            participanteExistente.UrlFoto = participante.UrlFoto;

            await _repository.UpdateAsync(participanteExistente);
        }

        public async Task<Participante?> ObtenerDashboardAsync(int id)
        {
            // Usamos la Mega-Consulta que preparamos en el repositorio específico
            return await _repository.ObtenerConDetallesDashboardAsync(id);
        }
    }
}