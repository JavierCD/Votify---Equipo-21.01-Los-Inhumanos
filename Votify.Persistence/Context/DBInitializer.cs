using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
            //context.Database.EnsureCreated();
            context.Database.Migrate();

            if (context.Eventos.Any())
            {
                return;
            }

            // ==========================================
            // 1. MIEMBROS (Organizadores, Jueces, Participantes)
            // ==========================================

            // 1.1 Organizadores (Mínimo 2)
            var org1 = new Organizador { Name = "Alan Brito (Org 1)", Email = "alan.brito@votify.com", Password = "HashFalsoTemporal123" };
            var org2 = new Organizador { Name = "Laura Campos (Org 2)", Email = "laura.campos@votify.com", Password = "HashFalsoTemporal123" };

            // 1.2 Jueces
            var juez1 = new Juez { Name = "Armando Bronca", Email = "juez1@votify.com", Password = "HashTemporal123" };
            var juez2 = new Juez { Name = "Elena Fuerte", Email = "juez2@votify.com", Password = "HashTemporal123" };

            // 1.3 Participantes (Creamos 15 participantes para repartirlos en los proyectos)
            var participantes = new List<Participante>();
            for (int i = 1; i <= 15; i++)
            {
                participantes.Add(new Participante
                {
                    Name = $"Equipo {i}",
                    Email = $"equipo{i}@votify.com",
                    Password = "HashTemporal123"
                });
            }

            context.Miembros.AddRange(org1, org2, juez1, juez2);
            context.Miembros.AddRange(participantes);
            context.SaveChanges();

            // ==========================================
            // 2. EVENTOS (2 por organizador = 4 eventos en total)
            // ==========================================
            EventoCreator creadorHackathon = new HackathonEventCreator();
            // Asumiendo que usas las fábricas, reutilizaremos HackathonEventCreator o instancias directas según tu arquitectura

            var evento1 = creadorHackathon.CrearEvento("Hackathon de Innovación 2026", DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(7), org1.Id, "Evento principal de prueba Org 1.");
            var evento2 = creadorHackathon.CrearEvento("Feria de Startups Tecnológicas", DateTime.UtcNow.AddDays(10), DateTime.UtcNow.AddDays(12), org1.Id, "Segundo evento de Org 1.");
            var evento3 = creadorHackathon.CrearEvento("Torneo eSports & Dev", DateTime.UtcNow.AddDays(15), DateTime.UtcNow.AddDays(18), org2.Id, "Primer evento de Org 2.");
            var evento4 = creadorHackathon.CrearEvento("Congreso de Ciberseguridad", DateTime.UtcNow.AddDays(20), DateTime.UtcNow.AddDays(22), org2.Id, "Segundo evento de Org 2.");

            // Asignamos el juez 1 al evento 1 para tus tests de notificaciones
            evento1.Jurado = new List<Juez> { juez1 };

            context.Eventos.AddRange(evento1, evento2, evento3, evento4);
            context.SaveChanges();

            // ==========================================
            // 3. CATEGORÍAS (Mínimo 2 por evento = 8 categorías)
            // ==========================================
            var categorias = new List<Categoria>
            {
                // Evento 1
                new Categoria { Name = "Impacto Social", Descripcion = "Soluciones para la sociedad.", EventoId = evento1.Id },
                new Categoria { Name = "Innovación en IA", Descripcion = "Uso de Inteligencia Artificial.", EventoId = evento1.Id },
                // Evento 2
                new Categoria { Name = "FinTech", Descripcion = "Tecnología financiera.", EventoId = evento2.Id },
                new Categoria { Name = "GreenTech", Descripcion = "Sostenibilidad y medio ambiente.", EventoId = evento2.Id },
                // Evento 3
                new Categoria { Name = "Desarrollo de Videojuegos", Descripcion = "Creación de entretenimiento.", EventoId = evento3.Id },
                new Categoria { Name = "Herramientas eSports", Descripcion = "Software para competición.", EventoId = evento3.Id },
                // Evento 4
                new Categoria { Name = "Seguridad Ofensiva", Descripcion = "Red Team y Pentesting.", EventoId = evento4.Id },
                new Categoria { Name = "Seguridad Defensiva", Descripcion = "Blue Team y Protección.", EventoId = evento4.Id }
            };

            context.Categorias.AddRange(categorias);
            context.SaveChanges();

            // ==========================================
            // 4. PROYECTOS (Mínimo 5 por categoría = 40 proyectos)
            // ==========================================
            var proyectos = new List<Proyecto>();
            int participanteIndex = 0;

            // Función local rápida para rotar a través de los 15 participantes disponibles
            Participante GetNextParticipante()
            {
                var p = participantes[participanteIndex];
                participanteIndex = (participanteIndex + 1) % participantes.Count;
                return p;
            }

            // Generar 5 proyectos para cada una de las 8 categorías
            for (int i = 0; i < categorias.Count; i++)
            {
                var categoriaActual = categorias[i];

                for (int j = 1; j <= 5; j++)
                {
                    Proyecto nuevoProyecto;
                    var participante = GetNextParticipante();

                    // Alternamos el tipo de proyecto instanciado basándonos en tu modelo
                    if (i % 3 == 0)
                    {
                        nuevoProyecto = new SustainabilityProject($"Sustain Proy {j} - Cat {i + 1}", participante.Id) { Visible = true };
                    }
                    else if (i % 3 == 1)
                    {
                        nuevoProyecto = new AiProject($"AI Proy {j} - Cat {i + 1}", participante.Id) { Visible = true };
                    }
                    else
                    {
                        nuevoProyecto = new CybersecurityProject($"Cyber Proy {j} - Cat {i + 1}", participante.Id) { Visible = true };
                    }

                    nuevoProyecto.Categorias.Add(categoriaActual);
                    proyectos.Add(nuevoProyecto);
                }
            }

            context.Proyectos.AddRange(proyectos);
            context.SaveChanges();

            // ==========================================
            // 5. PREPARACIÓN PARA TEST DE NOTIFICACIONES (Mantenido de tu código original)
            // ==========================================

            // Usamos la categoría "Innovación en IA" (que es la categorias[1] de nuestra lista)
            var votacionTest = new Popular
            {
                CategoriaId = categorias[1].Id,
                FechaApertura = DateTime.UtcNow.AddMinutes(1), // ¡El cron la detectará en breve!
                FechaCierre = DateTime.UtcNow.AddDays(2),
                Estado = "Pendiente", // Aún no ha empezado
                MaxSelection = 3,
                EnviarNotificacionApertura = true,
                NotificacionAperturaEnviada = false
            };

            context.Votaciones.Add(votacionTest);
            context.SaveChanges();
        }
    }
}