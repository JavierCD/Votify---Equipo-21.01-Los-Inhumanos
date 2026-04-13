using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Enums;
using Votify.Core.Factories;
using Votify.Core.Models;
using Votify.Persistence.Context;

namespace Votify.Persistence.Context
{
    public static class DbInitializer
    {
        public static void Initialize(VotifyContext context)
        {
            context.Database.EnsureDeleted();
            
            
            
            context.Database.EnsureCreated();

            if (context.Eventos.Any())
            {
                return;
            }

            // 1. Miembros
            var organizadorMock = new Organizador
            {
                Name = "Alan Brito (Organizador Demo)",
                Email = "alan.brito@votify.com",
                Password = "HashFalsoTemporal123"
            };

            var juez = new Juez
            {
                Name = "Armando Bronca (Juez)",
                Email = "juez@votify.com",
                Password = "HashTemporal123"
            };

            var juez2 = new Juez
            {
                Name = "Elena Fuerte (Juez)",
                Email = "elena.fuerte@votify.com",
                Password = "HashTemporal123"
            };

            var participante = new Participante
            {
                Name = "Paco Merte (Participante)",
                Email = "participante@votify.com",
                Password = "HashTemporal123"
            };

            var participante2 = new Participante
            {
                Name = "Equipo Alpha",
                Email = "alpha@votify.com",
                Password = "HashTemporal123"
            };

            var participante3 = new Participante
            {
                Name = "Equipo Beta",
                Email = "beta@votify.com",
                Password = "HashTemporal123"
            };

            var participante4 = new Participante
            {
                Name = "Equipo Gamma",
                Email = "gamma@votify.com",
                Password = "HashTemporal123"
            };

            var participante5 = new Participante
            {
                Name = "Equipo Delta",
                Email = "delta@votify.com",
                Password = "HashTemporal123"
            };

            var participante6 = new Participante
            {
                Name = "Equipo Omega",
                Email = "omega@votify.com",
                Password = "HashTemporal123"
            };

            var participante7 = new Participante
            {
                Name = "Equipo Sigma",
                Email = "sigma@votify.com",
                Password = "HashTemporal123"
            };

            var participante8 = new Participante
            {
                Name = "Equipo Zeta",
                Email = "zeta@votify.com",
                Password = "HashTemporal123"
            };

            context.Miembros.AddRange(organizadorMock, juez, juez2, participante, participante2, participante3, participante4, participante5, participante6, participante7, participante8);
            context.SaveChanges();

            // 2. Eventos 
            EventoCreator creadorHackathon = new HackathonEventCreator();
            Evento eventoDemo = creadorHackathon.CrearEvento(
                "Hackathon de Innovación 2026",
                DateTime.UtcNow.AddDays(5),
                DateTime.UtcNow.AddDays(7),
                organizadorMock.Id,
                "Evento principal de prueba."
            );

            context.Eventos.Add(eventoDemo);
            context.SaveChanges();

            // 3. Categorías del evento 1
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

            var categoria3 = new Categoria
            {
                Name = "Ciberseguridad",
                Descripcion = "Proyectos enfocados en seguridad digital.",
                EventoId = eventoDemo.Id
            };

            context.Categorias.AddRange(categoria1, categoria2, categoria3);
            context.SaveChanges();

            // 4. Proyectos asociados a categorías
            
            var proyecto1 = new SustainabilityProject("AgroTech Social", participante.Id)
            {
                Visible = true,
                
            };

            var proyecto2 = new SustainabilityProject("EduAccess", participante2.Id)
            {
                Visible = true,
                
            };

            var proyecto3 = new SustainabilityProject("GreenCity", participante3.Id)
            {
                Visible = true,
                
            };

            proyecto1.Categorias.Add(categoria1);
            proyecto2.Categorias.Add(categoria1);
            proyecto3.Categorias.Add(categoria1);

            var proyecto4 = new AiProject("HealthBot AI", participante4.Id)
            {
                Visible = true,
                
            };

            var proyecto5 = new AiProject("SmartCity Assistant", participante5.Id)
            {
                Visible = true,
                
            };

            var proyecto6 = new AiProject("EduBot", participante6.Id)
            {
                Visible = true,
                
            };

            proyecto4.Categorias.Add(categoria2);
            proyecto5.Categorias.Add(categoria2);
            proyecto6.Categorias.Add(categoria2);

            var proyecto7 = new CybersecurityProject("SecureVote", participante7.Id)
            {
                Visible = true,

            };

            var proyecto8 = new CybersecurityProject("CryptoShield", participante8.Id)
            {
                Visible = true,

            };

            proyecto7.Categorias.Add(categoria3); 
            proyecto8.Categorias.Add(categoria3);

            context.Proyectos.AddRange(proyecto1, proyecto2, proyecto3, proyecto4, proyecto5, proyecto6, proyecto7, proyecto8);
            context.SaveChanges();
            

            // 5. Votantes
            /*
            var votante1 = new Votante { Email = "votante1@votify.com", Anonimo = false, Rol = "PUBLIC" };
            var votante2 = new Votante { Email = "votante2@votify.com", Anonimo = false, Rol = "PUBLIC" };
            var votante3 = new Votante { Email = "votante3@votify.com", Anonimo = false, Rol = "PUBLIC" };
            var votante4 = new Votante { Email = "votante4@votify.com", Anonimo = false, Rol = "SPONSOR" };
            var votante5 = new Votante { Email = "votante5@votify.com", Anonimo = true, Rol = "PUBLIC" };
            var votante6 = new Votante { Email = "votante6@votify.com", Anonimo = false, Rol = "EXPERT" };

            context.Votantes.AddRange(votante1, votante2, votante3, votante4, votante5, votante6);
            context.SaveChanges();*/
        }
    }
}