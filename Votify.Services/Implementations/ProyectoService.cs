using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Interfaces;
using Votify.Core.Models;

namespace Votify.Services.Implementations
{
    public class ProyectoService : IProyectoService
    {
        private readonly IGenericRepository<Proyecto> _proyectoRepo;
        private readonly IGenericRepository<Categoria> _categoriaRepo;

        public ProyectoService(IGenericRepository<Proyecto> proyectoRepo, IGenericRepository<Categoria> categoriaRepo)
        {
            _proyectoRepo = proyectoRepo;
            _categoriaRepo = categoriaRepo;
        }

        public async Task CrearProyectoConCategoriaAsync(Proyecto proyecto, int categoriaId)
        {
            // 1. Buscamos la categoría real en la BD para que Entity Framework la rastree
            var categoria = await _categoriaRepo.GetByIdAsync(categoriaId);

            if (categoria != null)
            {
                // 2. Usamos tu método guardián para vincularla
                proyecto.AgregarCategoria(categoria);
            }

            // 3. Guardamos el proyecto (EF Core guardará la relación automáticamente)
            await _proyectoRepo.AddAsync(proyecto);
        }
    }
}
