using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Models
{
    public class Juez : Miembro
    {
        public Juez() : base() { }
        public Juez(string name, string email, string password) : base(name, email, password)
        {

        }
    }
}
