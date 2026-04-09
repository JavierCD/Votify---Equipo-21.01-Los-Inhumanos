using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Models
{
    public abstract class Miembro
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        // Asumimos que aquí guardarás el hash de la contraseña, no el texto plano 
        public string Password { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        


    }
}
