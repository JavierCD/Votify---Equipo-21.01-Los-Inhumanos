using Microsoft.EntityFrameworkCore;
using Votify.Core.Factories;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Interfaces;
using Votify.Services.Models;

namespace Votify.Services.Implementations
{
    public class VotoMulticriterioService : IVotoMulticriterioService
    {
        private readonly IGenericRepository<Multicriterio> _multicriterioRepository;
        private readonly IGenericRepository<Votante> _votanteRepository;
        private readonly IGenericRepository<Voto> _votoRepository;

        public VotoMulticriterioService(
            IGenericRepository<Multicriterio> multicriterioRepository,
            IGenericRepository<Votante> votanteRepository,
            IGenericRepository<Voto> votoRepository)
        {
            _multicriterioRepository = multicriterioRepository;
            _votanteRepository = votanteRepository;
            _votoRepository = votoRepository;
        }

        public async Task<List<VotacionMulticriterioDetalleResponse>> ObtenerVotacionesMulticriterioDisponiblesAsync()
        {
            // Devuelve todas las votaciones abiertas
            var votaciones = await _multicriterioRepository.GetAllAsync()
                .Include(v => v.Categoria)
                .Where(v => v.Estado == "Abierta")
                .ToListAsync();

            return votaciones.Select(v => new VotacionMulticriterioDetalleResponse
            {
                VotacionId = v.Id,
                CategoriaNombre = v.Categoria?.Name ?? "Sin Categoría",
                Estado = v.Estado
            }).ToList();
        }

        public async Task<List<VotacionMulticriterioDetalleResponse>> ObtenerVotacionesDisponiblesAsync(int votanteId)
        {
            // Similar al anterior, pero podrías aplicar lógica para ocultar las que este votante ya votó
            return await ObtenerVotacionesMulticriterioDisponiblesAsync();
        }

        public async Task<VotacionMulticriterioDetalleResponse> ObtenerDetallePorIdAsync(int votacionId)
        {
            // Obtenemos la votación con sus relaciones
            var votacion = await _multicriterioRepository.GetAllAsync()
                .Include(v => v.Categoria)
                    .ThenInclude(c => c.Proyectos)
                .Include(v => v.Criterios)
                .FirstOrDefaultAsync(v => v.Id == votacionId);

            if (votacion == null)
                throw new Exception("La votación multicriterio no existe.");

            return new VotacionMulticriterioDetalleResponse
            {
                VotacionId = votacion.Id,
                CategoriaNombre = votacion.Categoria?.Name ?? "Categoría Sin Nombre",
                Estado = votacion.Estado,
                Proyectos = votacion.Categoria?.Proyectos?
                    .Where(p => p.Visible)
                    .Select(p => new VotacionMulticriterioDetalleResponse.ProyectoDto
                    {
                        Id = p.Id,
                        Name = p.Name
                    }).ToList() ?? new(),
                Criterios = votacion.Criterios?
                    .Select(c => new VotacionMulticriterioDetalleResponse.CriterioBaremoDto
                    {
                        Id = c.Id,
                        Nombre = c.Name,
                        Peso = c.Peso
                    }).ToList() ?? new()
            };
        }

        public async Task EmitirVotoMulticriterioAsync(EmitirVotoMulticriterioRequest request)
        {
            // ==========================================
            // PATRÓN DOBLE BÓVEDA (ANONIMATO)
            // ==========================================

            // 1. Validar Doble Bóveda: Comprobar si el Email ya votó
            var votantes = await _votanteRepository.GetAllAsync();
            var yaVoto = votantes.Any(v => v.Email == request.Email /* && v.VotacionId == request.VotacionId */);

            if (yaVoto) throw new Exception("Este correo ya ha emitido una evaluación para esta categoría.");

            // 2. PATRÓN FACTORY PARA CREAR LA PAPELETA BASE
            VotoCreator creador = new VotoPublicoCreator();
            Voto papeleta = creador.CrearVoto();

            papeleta.VotacionId = request.VotacionId;
            papeleta.Fecha = DateTime.UtcNow; // Ajusta a 'FechaEmision' si así se llama en tu Core

            if (papeleta.Detalles == null)
                papeleta.Detalles = new List<DetalleVoto>();

            // 3. RELLENAR LA PAPELETA CON LAS LÍNEAS DE EVALUACIÓN
            foreach (var proyectoEvaluado in request.Puntuaciones)
            {
                int proyectoId = proyectoEvaluado.Key;
                foreach (var criterioEvaluado in proyectoEvaluado.Value)
                {
                    if (criterioEvaluado.Value > 0) // Guardamos si la nota es mayor a 0
                    {
                        papeleta.Detalles.Add(new DetalleVoto
                        {
                            ProyectoId = proyectoId,
                            CriterioId = criterioEvaluado.Key,
                            Puntuacion = criterioEvaluado.Value
                        });
                    }
                }
            }

            // 4. GUARDAR VOTO ANÓNIMO
            if (papeleta.Detalles.Any())
            {
                await _votoRepository.AddAsync(papeleta);
            }

            // 5. REGISTRAR VOTANTE PARA BLOQUEAR FUTUROS INTENTOS
            var registroVotante = new Votante
            {
                Email = request.Email,
                Anonimo = true,
                votacionId = request.VotacionId
            };

            await _votanteRepository.AddAsync(registroVotante);
        }
    }
}