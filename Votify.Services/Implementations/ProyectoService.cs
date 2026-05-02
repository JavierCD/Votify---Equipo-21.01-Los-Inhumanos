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
    }
}
