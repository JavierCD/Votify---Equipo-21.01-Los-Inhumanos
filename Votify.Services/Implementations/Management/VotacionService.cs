using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Enums;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Interfaces;

namespace Votify.Services.Implementations
{
    public class VotacionService : IVotacionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVotacionStateSubject _subject;

        public VotacionService(IUnitOfWork unitOfWork, IVotacionStateSubject subject)
        {
            _unitOfWork = unitOfWork;
            _subject = subject;
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
            var votacion = await _unitOfWork.Votaciones.GetWithIncludesAsync(
                v => v.Id == votacionId,
                v => v.Categoria,
                v => v.Categoria.Evento,
                v => v.Categoria.Evento.Jurado
            );

            if (votacion == null) return false;

            var evento = votacion.Categoria?.Evento;

            try
            {
                switch (nuevoEstado)
                {
                    case "Abierta":
                        votacion.ForzarApertura();
                        if (evento != null)
                        {
                            await _subject.NotifyAsync(new VotacionStateChangedArgs
                            {
                                Votacion = votacion,
                                Evento = evento,
                                EventType = VotacionStateEventType.Apertura,
                                TriggeredAt = DateTime.UtcNow
                            });
                        }
                        break;

                    case "Cerrada":
                        votacion.ForzarCierre();
                        if (evento != null)
                        {
                            await _subject.NotifyAsync(new VotacionStateChangedArgs
                            {
                                Votacion = votacion,
                                Evento = evento,
                                EventType = VotacionStateEventType.Cierre,
                                TriggeredAt = DateTime.UtcNow
                            });
                        }
                        break;

                    case "Pausada":
                        votacion.PausarVotacion();
                        break;

                    case "Programada":
                        votacion.ForzarProgramada();
                        break;
                }
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ActualizarVisibilidadVotacionAsync(int votacionId, bool mostrarJueces, bool mostrarComentarios, bool mostrarRanking, bool mostrarDetalles)
        {
            // Cargamos la categoría que incluye la votación para poder acceder a ella de forma segura
            var categoria = await _unitOfWork.CategoriaRepository.GetWithIncludesAsync(
                c => c.Votacion != null && c.Votacion.Id == votacionId,
                c => c.Votacion
            );

            if (categoria == null || categoria.Votacion == null) return false;

            // Asignamos los nuevos permisos al modelo de dominio
            categoria.Votacion.MostrarNombresJueces = mostrarJueces;
            categoria.Votacion.MostrarComentarios = mostrarComentarios;
            categoria.Votacion.MostrarRanking = mostrarRanking;
            categoria.Votacion.MostrarResultadosDetallados = mostrarDetalles;

            // Persistimos en PostgreSQL
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
