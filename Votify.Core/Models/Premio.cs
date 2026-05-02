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


        public string Name { get; set; }


        public string? Descripcion { get; set; }

        public int Posicion { get; set; }
        public bool PermiteEmpate { get; set; }

        public int CategoriaId { get; set; }

        public Categoria? Categoria { get; set; }

        protected Premio() { }

        public Premio(string nombre, string? desc, int puesto, bool permiteEmpate = false)
        {
            Name = nombre;
            Descripcion = desc;
            Posicion = puesto;
            PermiteEmpate = permiteEmpate;
        }
    }
}
