using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Votify.Persistence.Entities; // Asegúrate de que esta carpeta exista

namespace Votify.Persistence.Context
{
    public class VotifyContext : DbContext
    {
        public VotifyContext(DbContextOptions<VotifyContext> options) : base(options)
        {
        }

        // Aquí definiremos las tablas de tu base de datos
        // public DbSet<Evento> Eventos { get; set; }
    }
}