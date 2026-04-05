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

            context.Miembros.AddRange(organizadorMock, juez, participante);
            context.SaveChanges();

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
