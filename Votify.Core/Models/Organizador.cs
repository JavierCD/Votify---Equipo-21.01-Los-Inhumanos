using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Models
{
    public class Organizador : Miembro
    {
        public Organizador() : base() { }
        public Organizador(string name, string email, string password) : base(name, email, password)
        {

        }
    }
}
