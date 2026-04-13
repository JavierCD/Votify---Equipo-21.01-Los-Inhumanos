using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models;
using Votify.Core.Interfaces;
using Votify.Services.Interfaces;

namespace Votify.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<Miembro> _miembroRepository;
        public AuthService(IGenericRepository<Miembro> miembroRepository)
        {
            _miembroRepository = miembroRepository;
        }
        public async Task<Miembro> Login(string email, string password)
        {
            // Aquí deberías implementar la lógica de autenticación, por ejemplo:
            // 1. Buscar el miembro por su correo electrónico.
            // 2. Verificar que la contraseña sea correcta (esto debería hacerse con hashing y salting en un entorno real).
            // 3. Devolver el miembro si la autenticación es exitosa o lanzar una excepción si falla.
            var miembros = await _miembroRepository.GetAllAsync();
            var miembro = miembros.FirstOrDefault(m => m.Email == email);

            if (miembro == null || miembro.Password != password)
            {
                throw new UnauthorizedAccessException("Credenciales incorrectas .");
            }
            return miembro;
        }
        public async Task<Miembro> Register(string name, string email, string password, string role)
        {
            var miembros = await _miembroRepository.GetAllAsync();

            if (miembros.Any(m => m.Email == email))
                throw new Exception("El usuario ya existe");

            Miembro nuevoUsuario;

            switch (role)
            {
                case "Participante":
                    nuevoUsuario = new Participante(name, email, password);
                    break;

                case "Juez":
                    nuevoUsuario = new Juez(name, email, password);
                    break;

                case "Admin":
                    nuevoUsuario = new Organizador(name, email, password);
                    break;

                default:
                    throw new Exception("Rol inválido");
            }

            await _miembroRepository.AddAsync(nuevoUsuario);

            return nuevoUsuario;
        }
    }
}
