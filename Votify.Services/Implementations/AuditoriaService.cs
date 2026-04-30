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
        private readonly IAuditoriaRepository _auditoriaRepository;

        public AuditoriaService(IAuditoriaRepository auditoriaRepository)
        {
            _auditoriaRepository = auditoriaRepository;
        }

        public async Task<List<AuditoriaVotoResponse>> ObtenerHistorialPorEventoAsync(int eventoId)
        {
            var votos = await _auditoriaRepository.ObtenerAuditoriaPorEventoAsync(eventoId);

            // Mapeamos de Entidad a DTO usando el polimorfismo que preparamos antes
            return votos.Select(v => new AuditoriaVotoResponse
            {
                VotoId = v.Id,
                FechaEmision = v.Fecha,
                ProyectoNombre = v.Proyecto?.Name ?? "Proyecto Desconocido o Eliminado",
                TipoVotacion = v.RolVotante(), // "EXPERT", "PUBLIC", "SPONSOR"
                EsAnonimo = v.Anonimo,
                IdentificadorVotante = v.ObtenerIdentificadorAuditoria() // Resuelve automáticamente quién es
            }).ToList();
        }
    }
}
