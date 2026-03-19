using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Models
{
    public class Premio
    {
        public int Id { get; set; }

        // Usamos required para garantizar que no haya premios sin nombre
        public string Name { get; set; }

        // Nullable (?) porque a veces un premio se explica por sí solo ("Primer Puesto")
        public string? Descripcion { get; set; }

        // Un entero para ordenar quién es 1º, 2º, 3º...
        public int Posicion { get; set; }

        // Clave foránea hacia la Categoría
        public int CategoriaId { get; set; }

        // Propiedad de navegación pura
        public Categoria? Categoria { get; set; }
    }
}
