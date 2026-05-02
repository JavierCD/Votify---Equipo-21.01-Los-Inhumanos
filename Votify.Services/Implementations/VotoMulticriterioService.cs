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
        // Inyectamos el repositorio específico que acabas de crear
        private readonly IVotoMulticriterioRepository _votoRepo;
        private readonly IGenericRepository<Votante> _votanteRepo;

        public VotoMulticriterioService(
            IVotoMulticriterioRepository votoRepo,
            IGenericRepository<Votante> votanteRepo)
        {
            _votoRepo = votoRepo;
            _votanteRepo = votanteRepo;
        }

        public async Task<List<VotacionMulticriterioDetalleResponse>> ObtenerVotacionesMulticriterioDisponiblesAsync()
        {
            // 1. Delegamos la obtención de datos a la capa de persistencia.
            // El repositorio ya filtra por Estado == "Abierta" e incluye la Categoría.
            var votaciones = await _votoRepo.ObtenerVotacionesMulticriterioDisponiblesAsync();

            // 2. Mapeamos las entidades de dominio a nuestro DTO de respuesta para la UI.
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
            var votacion = await _votoRepo.ObtenerVotacionMulticriterioPorIdAsync(votacionId);

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
            // 0. OBTENER LA VOTACIÓN Y VALIDAR REGLAS DE DOMINIO (VENTANA DE TIEMPO)
            // Recuperamos la entidad rica desde la base de datos
            var votacion = await _votoRepo.ObtenerVotacionMulticriterioPorIdAsync(request.VotacionId);

            if (votacion == null)
                throw new Exception("La votación especificada no existe.");

            // Evaluamos la regla de negocio pura de la máquina de estados.
            // Si la fecha actual no está dentro de la ventana de tiempo, lanzamos excepción.
            if (!votacion.PuedeVotar(DateTime.UtcNow))
                throw new InvalidOperationException("La votación no está abierta en este momento.");

            // 1. Validar Doble Bóveda: Comprobar si el Email ya votó usando el repositorio correcto
            var yaVoto = await _votoRepo.EmailYaVotoEnVotacionAsync(request.VotacionId, request.Email);
            if (yaVoto) throw new Exception("Este correo ya ha emitido una evaluación para esta categoría.");

            // 2. Configurar el Factory
            VotoCreator creador = new VotoPublicoCreator();
            List<Voto> votosAInsertar = new List<Voto>();

            // 3. RELLENAR UNA PAPELETA (VOTO) POR CADA PROYECTO
            foreach (var proyectoEvaluado in request.Puntuaciones)
            {
                int proyectoId = proyectoEvaluado.Key;

                // NOTA: La puntuación base se puede calcular aquí ponderando los criterios, o dejar en 0
                double puntuacionBase = 0;

                // Creamos un voto pasando los parámetros requeridos por el Factory
                Voto papeleta = creador.CrearVoto(
                    votacionId: request.VotacionId,
                    proyectoId: proyectoId,
                    puntuacionBase: puntuacionBase,
                    anonimo: request.Anonimo
                );

                // Rellenamos las líneas de detalle (criterios) de ESTE proyecto
                foreach (var criterioEvaluado in proyectoEvaluado.Value)
                {
                    if (criterioEvaluado.Value > 0)
                    {
                        papeleta.Detalles.Add(new DetalleVoto
                        {
                            ProyectoId = proyectoId,
                            CriterioId = criterioEvaluado.Key,
                            Puntuacion = criterioEvaluado.Value
                        });
                    }
                }

                if (papeleta.Detalles.Any())
                {
                    votosAInsertar.Add(papeleta);
                }
            }

            // 4. GUARDAR TODOS LOS VOTOS
            if (votosAInsertar.Any())
            {
                await _votoRepo.GuardarVotosAsync(votosAInsertar);
            }

            // 5. REGISTRAR VOTANTE PARA BLOQUEAR FUTUROS INTENTOS
            var registroVotante = new Votante
            {
                Email = request.Email
            };

            await _votanteRepo.AddAsync(registroVotante);
        }
    }
}