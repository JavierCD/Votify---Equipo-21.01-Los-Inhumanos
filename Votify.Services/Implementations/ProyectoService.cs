using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Services.Interfaces;
using Votify.Services.DTOs;

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

        public async Task<bool> ActualizarProyectoAsync(int proyectoId, int usuarioPeticionId, string rolUsuario, string nombre, string? descripcion, string? nombresEquipo, string? urlMateriales)
        {
            var proyecto = await _unitOfWork.Proyectos.GetByIdAsync(proyectoId);

            if (proyecto == null)
                throw new KeyNotFoundException("El proyecto solicitado no existe.");

            // REGLA DE NEGOCIO: Solo el Admin o el Creador (Participante) pueden editar
            if (rolUsuario != "Organizador" && proyecto.ParticipanteId != usuarioPeticionId)
            {
                throw new UnauthorizedAccessException("No tienes permisos para editar la información de este proyecto.");
            }

            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del proyecto es obligatorio.");

            // Actualizamos solo los campos permitidos
            proyecto.Name = nombre;
            proyecto.Description = descripcion;
            proyecto.NombresEquipo = nombresEquipo;
            proyecto.UrlMaterialesExternos = urlMateriales;

            // Guardamos en Base de Datos
            await _unitOfWork.Proyectos.UpdateAsync(proyecto);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<ProyectoEdicionDto?> ObtenerProyectoParaEdicionAsync(int proyectoId)
        {
            // 1. Buscamos el proyecto incluyendo la categoría
            var proyecto = await _unitOfWork.Proyectos.GetWithIncludesAsync(
                p => p.Id == proyectoId,
                p => p.Categorias
            );

            if (proyecto == null) return null;

            // 2. Mapeamos los datos básicos al DTO
            var dto = new ProyectoEdicionDto
            {
                Id = proyecto.Id,
                ParticipanteId = proyecto.ParticipanteId,
                Nombre = proyecto.Name,
                Descripcion = proyecto.Description,
                NombresEquipo = proyecto.NombresEquipo,
                UrlMateriales = proyecto.UrlMaterialesExternos,
                Especialidad = proyecto.CategoriaEspecialidad()
            };

            // 3. Resolvemos los datos de relaciones profundas (Evento y Jurado)
            var categoria = proyecto.Categorias?.FirstOrDefault();
            if (categoria != null)
            {
                dto.NombreCategoria = categoria.Name;

                // Buscamos el evento completo para sacar el email de administración
                // (Asumo que tienes un repositorio de eventos en tu UnitOfWork)
                var evento = await _unitOfWork.Eventos.GetWithIncludesAsync(
                    e => e.Id == categoria.EventoId,
                    e => e.Jurado // Incluimos el jurado/admins para sacar el email
                );

                if (evento != null)
                {
                    dto.NombreEvento = evento.Name;
                    dto.CorreoAdmin = evento.Jurado?.FirstOrDefault()?.Email ?? "admin@evento.com";
                }
            }

            // Devolvemos un paquete limpio y seguro sin exponer el modelo real
            return dto;
        }
    }
}
