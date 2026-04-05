using Microsoft.EntityFrameworkCore;
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
            context.Database.EnsureDeleted();
            context.Database.Migrate();

            // 1. Miembros base
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

            context.Miembros.AddRange(organizadorMock, juez, participante, participante2, participante3, participante4, participante5);
            context.SaveChanges();

            // 2. Evento
            var eventoDemo = new Evento
            {
                Name = "Hackathon de Innovación 2026",
                Description = "Evento de prueba para verificar la creación y categorías.",
                FechaInicio = DateTime.UtcNow.AddDays(5),
                FechaFin = DateTime.UtcNow.AddDays(7),
                OrganizadorId = organizadorMock.Id,
                Organizador = organizadorMock,
                Estado = EstadoEvento.Borrador
            };

            context.Eventos.Add(eventoDemo);
            context.SaveChanges();

            // 3. Categorías
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

            // 4. Proyectos asociados a categorías
            var proyecto1 = new Proyecto
            {
                Name = "AgroTech Social",
                Visible = true,
                ParticipanteId = participante.Id,
                Categorias = new List<Categoria> { categoria1 }
            };

            var proyecto2 = new Proyecto
            {
                Name = "EduAccess",
                Visible = true,
                ParticipanteId = participante2.Id,
                Categorias = new List<Categoria> { categoria1 }
            };

            var proyecto3 = new Proyecto
            {
                Name = "HealthBot AI",
                Visible = true,
                ParticipanteId = participante3.Id,
                Categorias = new List<Categoria> { categoria2 }
            };

            var proyecto4 = new Proyecto
            {
                Name = "SmartCity Assistant",
                Visible = true,
                ParticipanteId = participante4.Id,
                Categorias = new List<Categoria> { categoria2 }
            };

            context.Proyectos.AddRange(proyecto1, proyecto2, proyecto3, proyecto4);
            context.SaveChanges();

            // 5. Votantes
            var votante1 = new Votante { Email = "votante1@votify.com", Anonimo = false };
            var votante2 = new Votante { Email = "votante2@votify.com", Anonimo = false };
            var votante3 = new Votante { Email = "votante3@votify.com", Anonimo = true };

            context.Votantes.AddRange(votante1, votante2, votante3);
            context.SaveChanges();
        }
    }
}
