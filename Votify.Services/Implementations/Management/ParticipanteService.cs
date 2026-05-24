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
        public async Task AsignarProyectoACategoriaAsync(int proyectoId, int categoriaId)
        {
            var proyecto = await _unitOfWork.Proyectos.GetWithIncludesAsync(
                p => p.Id == proyectoId,
                p => p.Categorias);

            if (proyecto == null)
                throw new KeyNotFoundException($"No se encontró el proyecto con ID {proyectoId}");

            var categoria = await _unitOfWork.Categorias.GetByIdAsync(categoriaId);
            if (categoria == null)
                throw new KeyNotFoundException($"No se encontró la categoría con ID {categoriaId}");

            if (proyecto.Categorias.Any(c => c.Id == categoriaId))
                throw new InvalidOperationException("El proyecto ya está asignado a esta categoría");

            proyecto.Categorias.Add(categoria);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ReasignarProyectoACategoriaAsync(int proyectoId, int categoriaOrigenId, int categoriaDestinoId)
        {
            var proyecto = await _unitOfWork.Proyectos.GetWithIncludesAsync(
                p => p.Id == proyectoId,
                p => p.Categorias);

            if (proyecto == null)
                throw new KeyNotFoundException($"No se encontró el proyecto con ID {proyectoId}");

            var categoriaOrigen = await _unitOfWork.Categorias.GetByIdAsync(categoriaOrigenId);
            if (categoriaOrigen == null)
                throw new KeyNotFoundException($"No se encontró la categoría de origen con ID {categoriaOrigenId}");

            var categoriaDestino = await _unitOfWork.Categorias.GetByIdAsync(categoriaDestinoId);
            if (categoriaDestino == null)
                throw new KeyNotFoundException($"No se encontró la categoría de destino con ID {categoriaDestinoId}");

            if (!proyecto.Categorias.Any(c => c.Id == categoriaOrigenId))
                throw new InvalidOperationException("El proyecto no está asignado a la categoría de origen");

            if (proyecto.Categorias.Any(c => c.Id == categoriaDestinoId))
                throw new InvalidOperationException("El proyecto ya está asignado a la categoría de destino");

            proyecto.Categorias.Remove(categoriaOrigen);
            proyecto.Categorias.Add(categoriaDestino);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ReasignarProyectoAOtroEventoAsync(int proyectoId, int eventoDestinoId, int categoriaDestinoId)
        {
            var proyecto = await _unitOfWork.Proyectos.GetWithIncludesAsync(
                p => p.Id == proyectoId,
                p => p.Categorias);

            if (proyecto == null)
                throw new KeyNotFoundException($"No se encontró el proyecto con ID {proyectoId}");

            var eventoDestino = await _unitOfWork.Eventos.GetWithIncludesAsync(
                e => e.Id == eventoDestinoId,
                e => e.CategoriasEvento);

            if (eventoDestino == null)
                throw new KeyNotFoundException($"No se encontró el evento destino con ID {eventoDestinoId}");

            var categoriaDestino = eventoDestino.CategoriasEvento.FirstOrDefault(c => c.Id == categoriaDestinoId);
            if (categoriaDestino == null)
                throw new KeyNotFoundException($"No se encontró la categoría destino con ID {categoriaDestinoId} en el evento destino");

            if (proyecto.Categorias.Any(c => c.Id == categoriaDestinoId))
                throw new InvalidOperationException("El proyecto ya está asignado a esta categoría");

            proyecto.Categorias.Clear();
            proyecto.Categorias.Add(categoriaDestino);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<Participante>> ObtenerParticipantesNoEnCategoriaAsync(int eventoId, int categoriaId)
        {
            // 1. Verificar que la categoría existe
            var categoria = await _unitOfWork.Categorias.GetWithIncludesAsync(
                c => c.Id == categoriaId,
                c => c.Proyectos);

            if (categoria == null)
                throw new KeyNotFoundException($"No se encontró la categoría con ID {categoriaId}");

            // 2. Obtener TODOS los participantes del sistema
            var todosParticipantes = await _unitOfWork.Participantes.GetAllAsync();

            // 3. Obtener IDs de participantes que ya tienen proyecto en esta categoría
            var participantesEnCategoriaIds = categoria.Proyectos
                .Where(p => p.ParticipanteId > 0)
                .Select(p => p.ParticipanteId)
                .Distinct()
                .ToList();

            // 4. Filtrar: todos los participantes EXCEPTO los que ya están en esta categoría
            var participantesDisponibles = todosParticipantes
                .Where(p => !participantesEnCategoriaIds.Contains(p.Id))
                .ToList();

            return participantesDisponibles;
        }

        public async Task<IEnumerable<Proyecto>> ObtenerProyectosDisponiblesParaAsignarAsync(int participanteId, int eventoId, int categoriaId)
        {
            var participante = await _unitOfWork.Participantes.GetWithIncludesAsync(
                p => p.Id == participanteId,
                p => p.Proyectos);

            if (participante == null)
                throw new KeyNotFoundException($"No se encontró el participante con ID {participanteId}");

            var proyectosDelParticipante = participante.Proyectos.ToList();

            var categoria = await _unitOfWork.Categorias.GetWithIncludesAsync(
                c => c.Id == categoriaId,
                c => c.Proyectos);

            if (categoria == null)
                throw new KeyNotFoundException($"No se encontró la categoría con ID {categoriaId}");

            var proyectosDisponibles = proyectosDelParticipante
                .Where(p => !categoria.Proyectos.Any(cp => cp.Id == p.Id))
                .ToList();

            return proyectosDisponibles;
        }

        public async Task<IEnumerable<Evento>> ObtenerEventosDisponiblesParaReasignarAsync(int eventoActualId)
        {
            var eventos = await _unitOfWork.Eventos.GetAllAsync();
            return eventos.Where(e => e.Id != eventoActualId).ToList();
        }

        public async Task<IEnumerable<Categoria>> ObtenerCategoriasDeEventoAsync(int eventoId)
        {
            var evento = await _unitOfWork.Eventos.GetWithIncludesAsync(
                e => e.Id == eventoId,
                e => e.CategoriasEvento);

            if (evento == null)
                throw new KeyNotFoundException($"No se encontró el evento con ID {eventoId}");

            return evento.CategoriasEvento;
        }
    }
}