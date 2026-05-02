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
        private readonly IGenericRepository<Votacion> _votacionRepository;

        public VotacionService(IGenericRepository<Votacion> votacionRepository)
        {
            _votacionRepository = votacionRepository;
        }

        public async Task ActualizarFechasVotacionAsync(int votacionId, DateTime nuevaApertura, DateTime nuevoCierre)
        {
            var votacion = await _votacionRepository.GetByIdAsync(votacionId);
            if (votacion == null) throw new Exception("Votación no encontrada.");

            votacion.ConfigurarFechas(nuevaApertura, nuevoCierre);
            await _votacionRepository.UpdateAsync(votacion);
        }

        public async Task<bool> CambiarEstadoVotacionManualAsync(int votacionId, string nuevoEstado)
        {
            var votacion = await _votacionRepository.GetByIdAsync(votacionId);
            if (votacion == null) return false;

            // Transicionamos de estado delegando la responsabilidad a la Entidad
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

            await _votacionRepository.UpdateAsync(votacion);
            return true;
        }
    }
}
