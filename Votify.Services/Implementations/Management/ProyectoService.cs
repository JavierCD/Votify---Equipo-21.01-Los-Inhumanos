using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Interfaces;
using Votify.Services.Models.Requests;
using Votify.Services.Models.Responses;

namespace Votify.Services.Implementations
{
    public class ProyectoService : IProyectoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProyectoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CrearProyectoConCategoriaAsync(Proyecto proyecto, int categoriaId)
        {
            var categoria = await _unitOfWork.Categorias.GetByIdAsync(categoriaId);
            if (categoria != null)
            {
                proyecto.AgregarCategoria(categoria);
            }

            await _unitOfWork.Proyectos.AddAsync(proyecto);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Proyecto?> ObtenerPorIdAsync(int id)
        {
            return await _unitOfWork.Proyectos.GetByIdAsync(id);
        }

        // REFACTORIZADO: Ahora recibe el objeto Request
        public async Task<bool> ActualizarProyectoAsync(EditarProyectoRequest request, int usuarioPeticionId, string rolUsuario)
        {
            var proyecto = await _unitOfWork.Proyectos.GetByIdAsync(request.Id);

            if (proyecto == null)
                throw new KeyNotFoundException("El proyecto solicitado no existe.");

            // REGLA DE NEGOCIO: Solo el Organizador o el Creador pueden editar
            if (rolUsuario != "Organizador" && proyecto.ParticipanteId != usuarioPeticionId)
            {
                throw new UnauthorizedAccessException("No tienes permisos para editar este proyecto.");
            }

            if (string.IsNullOrWhiteSpace(request.Nombre))
                throw new ArgumentException("El nombre del proyecto es obligatorio.");

            // Actualizamos solo los campos permitidos del Request
            proyecto.Name = request.Nombre;
            proyecto.Description = request.Descripcion;
            proyecto.NombresEquipo = request.NombresEquipo;
            proyecto.UrlMaterialesExternos = request.UrlMateriales;

            await _unitOfWork.Proyectos.UpdateAsync(proyecto);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // REFACTORIZADO: Ahora devuelve un Response de solo lectura
        public async Task<EditarProyectoResponse?> ObtenerProyectoParaEdicionAsync(int proyectoId)
        {
            var proyecto = await _unitOfWork.Proyectos.GetWithIncludesAsync(
                p => p.Id == proyectoId,
                p => p.Participante
            );

            if (proyecto == null) return null;

            var categoria = proyecto.Categorias?.FirstOrDefault();

            Evento? evento = null;
            if (categoria != null)
            {
                evento = await _unitOfWork.Eventos.GetWithIncludesAsync(
                            e => e.Id == categoria.EventoId,
                            e => e.Jurado,
                            e => e.Organizador // <--- AQUÍ ESTÁ EL CAMBIO: Incluimos el organizador
                        );
            }

            // 4. Mapeo al Response
            return new EditarProyectoResponse
            {
                Id = proyecto.Id,
                ParticipanteId = proyecto.ParticipanteId,
                Nombre = proyecto.Name,
                Descripcion = proyecto.Description,
                Especialidad = proyecto.CategoriaEspecialidad(),
                NombresEquipo = proyecto.NombresEquipo,
                UrlMateriales = proyecto.UrlMaterialesExternos,

                NombreCategoria = categoria?.Name ?? "Sin categoría",
                NombreEvento = evento?.Name ?? "Evento no especificado",

                CorreoParticipante = proyecto.Participante?.Email ?? "participante@desconocido.com",

                CorreoAdmin = evento?.Organizador?.Email ?? "admin@evento.com",
            };
        }
    }
}