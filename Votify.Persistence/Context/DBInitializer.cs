using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Enums;
using Votify.Core.Models;
using Votify.Persistence.Context;

namespace Votify.Persistence.Context
{
    public static class DbInitializer
    {
        public static void Initialize(VotifyContext context)
        {
            // 1. Verificamos si ya hay algún organizador en la base de datos
            // Como usamos TPH, EF Core buscará automáticamente en la tabla Miembros donde Discriminator == "Organizador"
            if (context.Miembros.OfType<Organizador>().Any())
            {
                return;   // Ya hay datos, no hacemos nada para no duplicar
            } else
            {
                // 2. Creamos nuestro Organizador Mock (El usuario que usaremos por ahora)
                var organizadorMock = new Organizador
                {
                    Name = "Alan Brito (Organizador Demo)",
                    Email = "alan.brito@votify.com",
                    Password = "HashFalsoTemporal123" // Tus compañeros de Login cambiarán esto luego
                };

                context.Miembros.Add(organizadorMock);
                context.SaveChanges(); // Guardamos para que PostgreSQL le asigne un ID real

                // 3. Creamos un Evento de prueba asociado a ese Organizador
                var eventoDemo = new Evento
                {
                    Name = "Hackathon de Innovación 2026",
                    Description = "Evento de prueba para verificar la creación y categorías.",
                    FechaInicio = DateTime.UtcNow.AddDays(5),
                    FechaFin = DateTime.UtcNow.AddDays(7),
                    OrganizadorId = organizadorMock.Id,
                    Estado = EstadoEvento.Borrador
                };


                context.Eventos.Add(eventoDemo);
                context.SaveChanges();

                // 4. Le añadimos algunas Categorías al evento
                var categoria1 = new Categoria("Proyectos Sociales", "Soluciones tecnológicas con impacto social")
                {
                    Name = "Proyectos Sociales",
                    Descripcion = "Soluciones tecnológicas con impacto social.",
                    EventoId = eventoDemo.Id
                };

                var categoria2 = new Categoria
                {
                    Name = "Innovación en IA",
                    Descripcion = "Mejor uso de modelos de Inteligencia Artificial.",
                    EventoId = eventoDemo.Id
                };



                context.Categorias.AddRange(categoria1, categoria2);
                context.SaveChanges(); 
            }



                
        }
    }
}
