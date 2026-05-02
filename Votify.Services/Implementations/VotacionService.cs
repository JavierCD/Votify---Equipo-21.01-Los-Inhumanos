using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Interfaces;

namespace Votify.Services.Implementations
{
    public class VotacionService : IVotacionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VotacionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ActualizarFechasVotacionAsync(int votacionId, DateTime nuevaApertura, DateTime nuevoCierre)
        {
            var votacion = await _unitOfWork.Votaciones.GetByIdAsync(votacionId);
            if (votacion == null) throw new Exception("Votación no encontrada.");

            votacion.ConfigurarFechas(nuevaApertura, nuevoCierre);
            await _unitOfWork.Votaciones.UpdateAsync(votacion);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> CambiarEstadoVotacionManualAsync(int votacionId, string nuevoEstado)
        {
            var votacion = await _unitOfWork.Votaciones.GetByIdAsync(votacionId);
            if (votacion == null) return false;

            switch (nuevoEstado)
            {
                case "Abierta":
                    votacion.ForzarApertura();
                    break;
                case "Cerrada":
                    votacion.ForzarCierre();
                    break;
                case "Pausada":
                    votacion.PausarVotacion();
                    break;
                case "Programada":
                    votacion.ForzarProgramada();
                    break;
            }

            await _unitOfWork.Votaciones.UpdateAsync(votacion);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
