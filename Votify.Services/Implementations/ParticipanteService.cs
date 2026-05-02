using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Core.Models;

namespace Votify.Services.Implementations
{
    public class ParticipanteService : IParticipanteService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ParticipanteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Participante?> ObtenerPorIdAsync(int id)
        {
            return await _unitOfWork.ParticipanteRepository.GetWithIncludesAsync(
                p => p.Id == id,
                p => p.Proyectos);
        }

        public async Task<IEnumerable<Participante>> ObtenerTodosAsync()
        {
            return await _unitOfWork.Participantes.GetAllAsync();
        }

        public async Task ActualizarFichaAsync(Participante participante)
        {
            var participanteExistente = await _unitOfWork.ParticipanteRepository.GetByIdAsync(participante.Id);
            if (participanteExistente == null)
            {
                throw new KeyNotFoundException($"No se encontró el participante con ID {participante.Id}");
            }

            participanteExistente.Name = participante.Name;
            participanteExistente.ActualizarFicha(participante.Descripcion);

            participanteExistente.InstitucionEducativa = participante.InstitucionEducativa;
            participanteExistente.Intereses = participante.Intereses;
            participanteExistente.ColorFondo = participante.ColorFondo;
            participanteExistente.UrlFoto = participante.UrlFoto;

            await _unitOfWork.ParticipanteRepository.UpdateAsync(participanteExistente);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Participante?> ObtenerDashboardAsync(int id)
        {
            return await _unitOfWork.ParticipanteRepository.ObtenerConDetallesDashboardAsync(id);
        }
    }
}