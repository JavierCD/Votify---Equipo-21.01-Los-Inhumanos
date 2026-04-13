using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models;

namespace Votify.Services.Interfaces
{
    
    public interface IAuthService {
        Task <Miembro> Login (string email, string password);
        Task<Miembro> Register(string name, string email, string password, string role);
    }
    
}
