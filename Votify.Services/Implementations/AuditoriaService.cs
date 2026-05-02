using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Services.Interfaces;
using Votify.Services.Models;

namespace Votify.Services.Implementations
{
    public class AuditoriaService : IAuditoriaService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuditoriaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AuditoriaVotoResponse>> ObtenerHistorialPorEventoAsync(int eventoId)
        {
            var votos = await _unitOfWork.AuditoriaRepository.ObtenerAuditoriaPorEventoAsync(eventoId);

            return votos.Select(v => new AuditoriaVotoResponse
            {
                VotoId = v.Id,
                FechaEmision = v.Fecha,
                ProyectoNombre = v.Proyecto?.Name ?? "Proyecto Desconocido o Eliminado",
                TipoVotacion = v.RolVotante(),
                EsAnonimo = v.Anonimo,
                IdentificadorVotante = v.ObtenerIdentificadorAuditoria()
            }).ToList();
        }
    }
}
