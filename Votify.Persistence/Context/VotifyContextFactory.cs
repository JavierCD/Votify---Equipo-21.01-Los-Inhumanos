using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Votify.Persistence.Context
{
    // Esta clase solo se usa cuando lanzas comandos desde la consola (Add-Migration, Update-Database)
    public class VotifyContextFactory : IDesignTimeDbContextFactory<VotifyContext>
    {
        public VotifyContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<VotifyContext>();

            // Aquí ponemos la cadena de conexión de forma directa (cambia los datos por los tuyos)
            var connectionString = "Host=localhost;Database=VotifyDB;Username=postgres;Password=PASSWORD";

            optionsBuilder.UseNpgsql(connectionString);

            return new VotifyContext(optionsBuilder.Options);
        }
    }
}
