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
            context.Database.Migrate();

            if (context.Eventos.Any())
            {
                return;
            }

            // ==========================================
            // 1. MIEMBROS
            // ==========================================

            // 1.1 Organizadores
            var org1 = new Organizador { Name = "María García López", Email = "maria.garcia@votify.com", Password = "Admin123!" };
            var org2 = new Organizador { Name = "Carlos Rodríguez Martín", Email = "carlos.rodriguez@votify.com", Password = "Admin123!" };

            // 1.2 Jueces (5 jueces expertos)
            var juez1 = new Juez { Name = "Dr. Alejandro Fernández", Email = "alejandro.fernandez@votify.com", Password = "Juez123!", QuiereRecibirNotificaciones = true };
            var juez2 = new Juez { Name = "Dra. Isabel Martínez", Email = "isabel.martinez@votify.com", Password = "Juez123!", QuiereRecibirNotificaciones = true };
            var juez3 = new Juez { Name = "Prof. Roberto Sánchez", Email = "roberto.sanchez@votify.com", Password = "Juez123!", QuiereRecibirNotificaciones = true };
            var juez4 = new Juez { Name = "Laura Domínguez", Email = "laura.dominguez@votify.com", Password = "Juez123!", QuiereRecibirNotificaciones = true };
            var juez5 = new Juez { Name = "Miguel Ángel Torres", Email = "miguel.torres@votify.com", Password = "Juez123!", QuiereRecibirNotificaciones = true };

            // 1.3 Participantes (20 equipos con nombres reales)
            var participantes = new List<Participante>
            {
                new Participante { Name = "Equipo NovaTech", Email = "novatech@votify.com", Password = "Part123!" },
                new Participante { Name = "Equipo CodeBreakers", Email = "codebreakers@votify.com", Password = "Part123!" },
                new Participante { Name = "Equipo GreenSolutions", Email = "greensolutions@votify.com", Password = "Part123!" },
                new Participante { Name = "Equipo DataMinds", Email = "dataminds@votify.com", Password = "Part123!" },
                new Participante { Name = "Equipo CyberShield", Email = "cybershield@votify.com", Password = "Part123!" },
                new Participante { Name = "Equipo AI Pioneers", Email = "aipioneers@votify.com", Password = "Part123!" },
                new Participante { Name = "Equipo EcoInnovate", Email = "ecoinnovate@votify.com", Password = "Part123!" },
                new Participante { Name = "Equipo PixelForge", Email = "pixelforge@votify.com", Password = "Part123!" },
                new Participante { Name = "Equipo SecureNet", Email = "securenet@votify.com", Password = "Part123!" },
                new Participante { Name = "Equipo CloudNine", Email = "cloudnine@votify.com", Password = "Part123!" },
                new Participante { Name = "Equipo QuantumLeap", Email = "quantumleap@votify.com", Password = "Part123!" },
                new Participante { Name = "Equipo SmartCity", Email = "smartcity@votify.com", Password = "Part123!" },
                new Participante { Name = "Equipo BioTech Labs", Email = "biotechlabs@votify.com", Password = "Part123!" },
                new Participante { Name = "Equipo FinFlow", Email = "finflow@votify.com", Password = "Part123!" },
                new Participante { Name = "Equipo GameDev Studio", Email = "gamedevstudio@votify.com", Password = "Part123!" },
                new Participante { Name = "Equipo NeuralPath", Email = "neuralpath@votify.com", Password = "Part123!" },
                new Participante { Name = "Equipo SolarTech", Email = "solartech@votify.com", Password = "Part123!" },
                new Participante { Name = "Equipo RedWolf", Email = "redwolf@votify.com", Password = "Part123!" },
                new Participante { Name = "Equipo BlueOcean", Email = "blueocean@votify.com", Password = "Part123!" },
                new Participante { Name = "Equipo DeepLearn", Email = "deeplearn@votify.com", Password = "Part123!" }
            };

            context.Miembros.AddRange(org1, org2, juez1, juez2, juez3, juez4, juez5);
            context.Miembros.AddRange(participantes);
            context.SaveChanges();

            // ==========================================
            // 2. EVENTOS
            // ==========================================
            EventoCreator creadorHackathon = new HackathonEventCreator();
            EventoCreator creadorInnovation = new InnovationFairEventCreator();
            EventoCreator creadorESports = new ESportsEventCreator();

            var evento1 = creadorHackathon.CrearEvento(
                "Hackathon Innovación Digital 2026",
                DateTime.UtcNow.AddDays(5),
                DateTime.UtcNow.AddDays(7),
                org1.Id,
                "Evento principal de innovación tecnológica. 48 horas para crear soluciones que transformen la sociedad."
            );

            var evento2 = creadorInnovation.CrearEvento(
                "Startup Weekend FinTech & GreenTech",
                DateTime.UtcNow.AddDays(10),
                DateTime.UtcNow.AddDays(12),
                org1.Id,
                "Fin de semana intensivo para emprendedores en tecnología financiera y sostenibilidad."
            );

            var evento3 = creadorESports.CrearEvento(
                "GameJam & eSports Challenge",
                DateTime.UtcNow.AddDays(15),
                DateTime.UtcNow.AddDays(18),
                org2.Id,
                "Competición de desarrollo de videojuegos y herramientas para la industria eSports."
            );

            var evento4 = creadorHackathon.CrearEvento(
                "CyberSec Summit 2026",
                DateTime.UtcNow.AddDays(20),
                DateTime.UtcNow.AddDays(22),
                org2.Id,
                "Cumbre de ciberseguridad con competiciones de pentesting y defensa de sistemas."
            );

            // Asignar jueces a eventos
            evento1.Jurado = new List<Juez> { juez1, juez2, juez3 };
            evento2.Jurado = new List<Juez> { juez2, juez4 };
            evento3.Jurado = new List<Juez> { juez3, juez5 };
            evento4.Jurado = new List<Juez> { juez1, juez4, juez5 };

            context.Eventos.AddRange(evento1, evento2, evento3, evento4);
            context.SaveChanges();

            // ==========================================
            // 3. CATEGORÍAS
            // ==========================================
            var categorias = new List<Categoria>
            {
                // Evento 1: Hackathon Innovación Digital
                new Categoria { Name = "Impacto Social", Descripcion = "Proyectos que resuelven problemas reales de la sociedad mediante tecnología.", EventoId = evento1.Id },
                new Categoria { Name = "Inteligencia Artificial Aplicada", Descripcion = "Soluciones basadas en machine learning, NLP o computer vision.", EventoId = evento1.Id },

                // Evento 2: Startup Weekend
                new Categoria { Name = "FinTech Innovadora", Descripcion = "Productos financieros disruptivos: pagos, inversiones, blockchain.", EventoId = evento2.Id },
                new Categoria { Name = "GreenTech Sostenible", Descripcion = "Tecnología para la sostenibilidad: energía limpia, reciclaje, agricultura.", EventoId = evento2.Id },

                // Evento 3: GameJam & eSports
                new Categoria { Name = "Mejor Videojuego Indie", Descripcion = "Juegos originales desarrollados durante el evento.", EventoId = evento3.Id },
                new Categoria { Name = "Herramienta para eSports", Descripcion = "Software de análisis, streaming o gestión de equipos competitivos.", EventoId = evento3.Id },

                // Evento 4: CyberSec Summit
                new Categoria { Name = "Red Team & Pentesting", Descripcion = "Herramientas y técnicas de ataque ético y pruebas de penetración.", EventoId = evento4.Id },
                new Categoria { Name = "Blue Team & Defensa", Descripcion = "Sistemas de detección, monitorización y respuesta a incidentes.", EventoId = evento4.Id }
            };

            context.Categorias.AddRange(categorias);
            context.SaveChanges();

            // ==========================================
            // 4. PROYECTOS (nombres significativos)
            // ==========================================
            var proyectos = new List<Proyecto>();

            // Categoría 0: Impacto Social (Evento 1)
            var p0_0 = new SustainabilityProject("EducaApp", participantes[0].Id, 8.5, 7.2, "Plataforma de educación accesible para zonas rurales") { Visible = true };
            var p0_1 = new SustainabilityProject("SaludConnect", participantes[1].Id, 9.0, 8.1, "Telemedicina para comunidades sin acceso a hospitales") { Visible = true };
            var p0_2 = new SustainabilityProject("AguaLimpia", participantes[2].Id, 7.8, 9.0, "Sistema de purificación de agua con IoT") { Visible = true };
            var p0_3 = new SustainabilityProject("MovilidadInclusiva", participantes[3].Id, 8.2, 7.5, "App de transporte accesible para personas con discapacidad") { Visible = true };
            var p0_4 = new SustainabilityProject("ComidaCompartida", participantes[4].Id, 7.5, 8.8, "Red de distribución de excedentes alimentarios") { Visible = true };
            var cat0Projs = new Proyecto[] { p0_0, p0_1, p0_2, p0_3, p0_4 };
            proyectos.AddRange(cat0Projs);
            foreach (Proyecto p in cat0Projs)
            {
                p.Categorias.Add(categorias[0]);
                categorias[0].Proyectos.Add(p);
            }

            // Categoría 1: IA Aplicada (Evento 1)
            var p1_0 = new AiProject("MedDiagnose AI", participantes[5].Id, 9.2, 8.5, "Diagnóstico médico asistido por inteligencia artificial") { Visible = true };
            var p1_1 = new AiProject("TraductorSeñas", participantes[6].Id, 8.8, 9.1, "Traducción en tiempo real de lengua de signos mediante visión por computador") { Visible = true };
            var p1_2 = new AiProject("CropPredict", participantes[7].Id, 8.0, 7.9, "Predicción de cosechas usando datos satelitales y ML") { Visible = true };
            var p1_3 = new AiProject("VoiceAssist Senior", participantes[8].Id, 8.5, 8.3, "Asistente de voz adaptado para personas mayores") { Visible = true };
            var p1_4 = new AiProject("LegalBot", participantes[9].Id, 7.9, 8.0, "Chatbot de asesoría legal gratuita con NLP") { Visible = true };
            var cat1Projs = new Proyecto[] { p1_0, p1_1, p1_2, p1_3, p1_4 };
            proyectos.AddRange(cat1Projs);
            foreach (Proyecto p in cat1Projs)
            {
                p.Categorias.Add(categorias[1]);
                categorias[1].Proyectos.Add(p);
            }

            // Categoría 2: FinTech (Evento 2)
            var p2_0 = new SustainabilityProject("PayMicro", participantes[10].Id, 8.7, 7.5, "Micro-pagos instantáneos para creadores de contenido") { Visible = true };
            var p2_1 = new SustainabilityProject("CryptoSave", participantes[11].Id, 7.8, 8.2, "Ahorro automático con stablecoins para economías inflacionarias") { Visible = true };
            var p2_2 = new SustainabilityProject("CreditFair", participantes[12].Id, 8.3, 7.9, "Scoring crediticio alternativo con datos no tradicionales") { Visible = true };
            var p2_3 = new SustainabilityProject("SplitBill Pro", participantes[13].Id, 7.5, 7.0, "Gestión inteligente de gastos compartidos con IA") { Visible = true };
            var p2_4 = new SustainabilityProject("InsurTech Simple", participantes[14].Id, 8.0, 8.5, "Seguros bajo demanda con activación por geolocalización") { Visible = true };
            var cat2Projs = new Proyecto[] { p2_0, p2_1, p2_2, p2_3, p2_4 };
            proyectos.AddRange(cat2Projs);
            foreach (Proyecto p in cat2Projs)
            {
                p.Categorias.Add(categorias[2]);
                categorias[2].Proyectos.Add(p);
            }

            // Categoría 3: GreenTech (Evento 2)
            var p3_0 = new SustainabilityProject("SolarTrack", participantes[15].Id, 9.0, 8.8, "Seguimiento solar inteligente para paneles fotovoltaicos") { Visible = true };
            var p3_1 = new SustainabilityProject("RecyclaMap", participantes[16].Id, 8.2, 7.5, "Mapa colaborativo de puntos de reciclaje con gamificación") { Visible = true };
            var p3_2 = new SustainabilityProject("AirPure Monitor", participantes[17].Id, 8.8, 9.0, "Red de sensores de calidad del aire en tiempo real") { Visible = true };
            var p3_3 = new SustainabilityProject("EcoRoute", participantes[18].Id, 7.9, 8.1, "Rutas de transporte con menor huella de carbono") { Visible = true };
            var p3_4 = new SustainabilityProject("WaterWise", participantes[19].Id, 8.5, 8.7, "Gestión inteligente del riego agrícola con IoT") { Visible = true };
            var cat3Projs = new Proyecto[] { p3_0, p3_1, p3_2, p3_3, p3_4 };
            proyectos.AddRange(cat3Projs);
            foreach (Proyecto p in cat3Projs)
            {
                p.Categorias.Add(categorias[3]);
                categorias[3].Proyectos.Add(p);
            }

            // Categoría 4: Mejor Videojuego Indie (Evento 3)
            var p4_0 = new AiProject("Shadows of Code", participantes[0].Id, 9.0, 8.5, "Aventura gráfica sobre un programador atrapado en su propio código") { Visible = true };
            var p4_1 = new AiProject("PixelQuest RPG", participantes[1].Id, 8.5, 9.0, "RPG retro con mecánicas de crafting innovadoras") { Visible = true };
            var p4_2 = new AiProject("Neon Drift", participantes[2].Id, 8.8, 7.8, "Carreras futuristas con física de gravedad variable") { Visible = true };
            var p4_3 = new AiProject("Mystic Garden", participantes[3].Id, 7.5, 8.2, "Puzzle relajante ambientado en jardines japoneses") { Visible = true };
            var p4_4 = new AiProject("Void Runners", participantes[4].Id, 8.2, 8.0, "Roguelike cooperativo en el espacio profundo") { Visible = true };
            var cat4Projs = new Proyecto[] { p4_0, p4_1, p4_2, p4_3, p4_4 };
            proyectos.AddRange(cat4Projs);
            foreach (Proyecto p in cat4Projs)
            {
                p.Categorias.Add(categorias[4]);
                categorias[4].Proyectos.Add(p);
            }

            // Categoría 5: Herramienta eSports (Evento 3)
            var p5_0 = new AiProject("ScoutVision", participantes[5].Id, 9.1, 8.5, "Plataforma de análisis de rendimiento para equipos profesionales") { Visible = true };
            var p5_1 = new AiProject("StreamOverlay Pro", participantes[6].Id, 8.3, 7.9, "Overlays dinámicos para streamers con interacción de chat") { Visible = true };
            var p5_2 = new AiProject("TeamSync", participantes[7].Id, 8.0, 8.2, "Gestión de equipos y torneos con calendario inteligente") { Visible = true };
            var p5_3 = new AiProject("ReplayAnalyzer", participantes[8].Id, 8.7, 8.0, "Análisis automático de replays con IA para mejorar gameplay") { Visible = true };
            var p5_4 = new AiProject("FanEngage", participantes[9].Id, 7.8, 7.5, "Plataforma de predicciones y fantasy para espectadores") { Visible = true };
            var cat5Projs = new Proyecto[] { p5_0, p5_1, p5_2, p5_3, p5_4 };
            proyectos.AddRange(cat5Projs);
            foreach (Proyecto p in cat5Projs)
            {
                p.Categorias.Add(categorias[5]);
                categorias[5].Proyectos.Add(p);
            }

            // Categoría 6: Red Team & Pentesting (Evento 4)
            var p6_0 = new CybersecurityProject("PhishHunter", participantes[10].Id, 9.0, 8.5, "Detección de campañas de phishing con análisis de comportamiento") { Visible = true };
            var p6_1 = new CybersecurityProject("VulnMapper", participantes[11].Id, 8.5, 9.0, "Mapeo automático de vulnerabilidades en infraestructuras cloud") { Visible = true };
            var p6_2 = new CybersecurityProject("RedBot Framework", participantes[12].Id, 8.8, 8.2, "Framework automatizado de pruebas de penetración") { Visible = true };
            var p6_3 = new CybersecurityProject("PassCrack Audit", participantes[13].Id, 7.5, 7.8, "Auditoría de fortaleza de contraseñas corporativas") { Visible = true };
            var p6_4 = new CybersecurityProject("SocialEng Toolkit", participantes[14].Id, 8.2, 8.0, "Kit de evaluación de ingeniería social para empresas") { Visible = true };
            var cat6Projs = new Proyecto[] { p6_0, p6_1, p6_2, p6_3, p6_4 };
            proyectos.AddRange(cat6Projs);
            foreach (Proyecto p in cat6Projs)
            {
                p.Categorias.Add(categorias[6]);
                categorias[6].Proyectos.Add(p);
            }

            // Categoría 7: Blue Team & Defensa (Evento 4)
            var p7_0 = new CybersecurityProject("ThreatWatch SIEM", participantes[15].Id, 9.2, 8.8, "SIEM ligero con detección de anomalías en tiempo real") { Visible = true };
            var p7_1 = new CybersecurityProject("IncidentFlow", participantes[16].Id, 8.5, 8.0, "Plataforma de respuesta a incidentes con playbooks automatizados") { Visible = true };
            var p7_2 = new CybersecurityProject("NetShield IDS", participantes[17].Id, 8.8, 9.0, "Sistema de detección de intrusos basado en ML") { Visible = true };
            var p7_3 = new CybersecurityProject("LogForge", participantes[18].Id, 7.9, 8.2, "Centralización y análisis forense de logs multi-plataforma") { Visible = true };
            var p7_4 = new CybersecurityProject("ZeroTrust Gateway", participantes[19].Id, 8.5, 8.5, "Gateway de acceso zero-trust con autenticación continua") { Visible = true };
            var cat7Projs = new Proyecto[] { p7_0, p7_1, p7_2, p7_3, p7_4 };
            proyectos.AddRange(cat7Projs);
            foreach (Proyecto p in cat7Projs)
            {
                p.Categorias.Add(categorias[7]);
                categorias[7].Proyectos.Add(p);
            }

            context.Proyectos.AddRange(proyectos);
            context.SaveChanges();

            // ==========================================
            // 5. VOTACIONES DE EJEMPLO (todos los tipos)
            // ==========================================

            // 5.1 Votación Popular - abre en 1 minuto (para test de notificaciones)
            var votacionPopularTest = new Popular
            {
                CategoriaId = categorias[1].Id, // IA Aplicada
                FechaApertura = DateTime.UtcNow.AddMinutes(1),
                FechaCierre = DateTime.UtcNow.AddDays(3),
                Estado = "Pendiente",
                MaxSelection = 3,
                EnviarNotificacionApertura = true,
                NotificacionAperturaEnviada = false,
                PermiteAutoVoto = false,
                RestriccionVotoUnico = true
            };
            context.Votaciones.Add(votacionPopularTest);

            // 5.2 Votación Popular - ya abierta
            var votacionPopularAbierta = new Popular
            {
                CategoriaId = categorias[0].Id, // Impacto Social
                FechaApertura = DateTime.UtcNow.AddDays(-1),
                FechaCierre = DateTime.UtcNow.AddDays(5),
                Estado = "Abierta",
                MaxSelection = 2,
                EnviarNotificacionApertura = true,
                NotificacionAperturaEnviada = true,
                PermiteAutoVoto = true,
                RestriccionVotoUnico = false
            };
            context.Votaciones.Add(votacionPopularAbierta);

            // 5.3 Votación por Puntuación - abre pronto
            var votacionPuntuacion = new Puntuacion
            {
                CategoriaId = categorias[2].Id, // FinTech
                FechaApertura = DateTime.UtcNow.AddMinutes(2),
                FechaCierre = DateTime.UtcNow.AddDays(4),
                Estado = "Pendiente",
                ValorMax = 10,
                EnviarNotificacionApertura = true,
                NotificacionAperturaEnviada = false,
                PermiteAutoVoto = false,
                RestriccionVotoUnico = true
            };
            context.Votaciones.Add(votacionPuntuacion);

            // 5.4 Votación por Puntuación - ya abierta
            var votacionPuntuacionAbierta = new Puntuacion
            {
                CategoriaId = categorias[3].Id, // GreenTech
                FechaApertura = DateTime.UtcNow.AddDays(-2),
                FechaCierre = DateTime.UtcNow.AddDays(2),
                Estado = "Abierta",
                ValorMax = 100,
                EnviarNotificacionApertura = true,
                NotificacionAperturaEnviada = true,
                PermiteAutoVoto = false,
                RestriccionVotoUnico = true
            };
            context.Votaciones.Add(votacionPuntuacionAbierta);

            // 5.5 Votación Multicriterio - abre pronto
            var votacionMulticriterio = new Multicriterio
            {
                CategoriaId = categorias[4].Id, // Mejor Videojuego Indie
                FechaApertura = DateTime.UtcNow.AddMinutes(3),
                FechaCierre = DateTime.UtcNow.AddDays(6),
                Estado = "Pendiente",
                UsaPesos = true,
                EnviarNotificacionApertura = true,
                NotificacionAperturaEnviada = false,
                PermiteAutoVoto = false,
                RestriccionVotoUnico = true,
                Criterios = new List<Criterio>
                {
                    new Criterio { Name = "Jugabilidad", Peso = 35 },
                    new Criterio { Name = "Diseño Visual", Peso = 25 },
                    new Criterio { Name = "Innovación", Peso = 20 },
                    new Criterio { Name = "Narrativa", Peso = 20 }
                }
            };
            context.Votaciones.Add(votacionMulticriterio);

            // 5.6 Votación Multicriterio - ya abierta
            var votacionMulticriterioAbierta = new Multicriterio
            {
                CategoriaId = categorias[6].Id, // Red Team
                FechaApertura = DateTime.UtcNow.AddDays(-1),
                FechaCierre = DateTime.UtcNow.AddDays(4),
                Estado = "Abierta",
                UsaPesos = true,
                EnviarNotificacionApertura = true,
                NotificacionAperturaEnviada = true,
                PermiteAutoVoto = false,
                RestriccionVotoUnico = true,
                Criterios = new List<Criterio>
                {
                    new Criterio { Name = "Efectividad del Ataque", Peso = 40 },
                    new Criterio { Name = "Documentación", Peso = 30 },
                    new Criterio { Name = "Creatividad", Peso = 30 }
                }
            };
            context.Votaciones.Add(votacionMulticriterioAbierta);

            // ==========================================
            // 6. EVENTO SIN VOTACIONES (para que las crees tú)
            // ==========================================
            var evento5 = creadorInnovation.CrearEvento(
                "Demo Day Innovación 2026",
                DateTime.UtcNow.AddDays(30),
                DateTime.UtcNow.AddDays(32),
                org1.Id,
                "Evento de demostración sin votaciones configuradas. Crea tus propias votaciones desde el panel de administración."
            );
            evento5.Jurado = new List<Juez> { juez1, juez3, juez5 };

            context.Eventos.Add(evento5);
            context.SaveChanges();

            var catDemo1 = new Categoria { Name = "Mejor Prototipo", Descripcion = "Prototipos funcionales presentados en el Demo Day.", EventoId = evento5.Id };
            var catDemo2 = new Categoria { Name = "Mejor Pitch", Descripcion = "Mejor presentación y comunicación del proyecto.", EventoId = evento5.Id };
            context.Categorias.AddRange(catDemo1, catDemo2);
            context.SaveChanges();

            // Evento 5: Demo Day - 5 proyectos por categoría
            var demo1_0 = new AiProject("AutoScheduler AI", participantes[0].Id, 7.5, 8.0, "Generador automático de horarios con IA") { Visible = true };
            var demo1_1 = new AiProject("CodeReview Bot", participantes[3].Id, 7.8, 7.2, "Bot de revisiones de código automatizadas") { Visible = true };
            var demo1_2 = new SustainabilityProject("EcoTrack", participantes[1].Id, 8.0, 7.5, "Tracker de huella de carbono personal") { Visible = true };
            var demo1_3 = new CybersecurityProject("PhishGuard", participantes[2].Id, 8.5, 8.0, "Extensión anti-phishing para navegadores") { Visible = true };
            var demo1_4 = new AiProject("TaskMaster AI", participantes[5].Id, 8.2, 7.8, "Gestión inteligente de tareas con priorización automática") { Visible = true };
            var demoCat1 = new Proyecto[] { demo1_0, demo1_1, demo1_2, demo1_3, demo1_4 };
            foreach (Proyecto p in demoCat1)
            {
                p.Categorias.Add(catDemo1);
                catDemo1.Proyectos.Add(p);
            }

            var demo2_0 = new SustainabilityProject("SmartBin", participantes[4].Id, 8.2, 8.5, "Papelera inteligente con clasificación automática") { Visible = true };
            var demo2_1 = new SustainabilityProject("SolarHome", participantes[6].Id, 7.9, 8.0, "Monitor de consumo energético doméstico") { Visible = true };
            var demo2_2 = new AiProject("VoiceNotes AI", participantes[7].Id, 8.5, 7.5, "Transcripción y resumen automático de notas de voz") { Visible = true };
            var demo2_3 = new CybersecurityProject("PassVault", participantes[8].Id, 7.8, 8.2, "Gestor de contraseñas con autenticación biométrica") { Visible = true };
            var demo2_4 = new AiProject("PresentationPro", participantes[9].Id, 8.0, 8.5, "Generador automático de presentaciones desde texto") { Visible = true };
            var demoCat2 = new Proyecto[] { demo2_0, demo2_1, demo2_2, demo2_3, demo2_4 };
            foreach (Proyecto p in demoCat2)
            {
                p.Categorias.Add(catDemo2);
                catDemo2.Proyectos.Add(p);
            }

            context.Proyectos.AddRange(demoCat1.Concat(demoCat2));
            context.SaveChanges();

            // ==========================================
            // 7. SEGUNDO EVENTO SIN VOTACIONES
            // ==========================================
            var evento6 = creadorESports.CrearEvento(
                "Tech Showcase Primavera 2026",
                DateTime.UtcNow.AddDays(45),
                DateTime.UtcNow.AddDays(47),
                org2.Id,
                "Escaparate tecnológico de primavera. Sin votaciones configuradas inicialmente."
            );
            evento6.Jurado = new List<Juez> { juez2, juez4 };

            context.Eventos.Add(evento6);
            context.SaveChanges();

            var catTech1 = new Categoria { Name = "Innovación Disruptiva", Descripcion = "Proyectos que cambian las reglas del juego.", EventoId = evento6.Id };
            var catTech2 = new Categoria { Name = "Mejor UX/UI", Descripcion = "Mejor experiencia de usuario e interfaz visual.", EventoId = evento6.Id };
            context.Categorias.AddRange(catTech1, catTech2);
            context.SaveChanges();

            // Evento 6: Tech Showcase - 5 proyectos por categoría
            var tech1_0 = new AiProject("MindMap AI", participantes[10].Id, 8.0, 7.8, "Generador de mapas mentales con IA generativa") { Visible = true };
            var tech1_1 = new CybersecurityProject("ZeroTrust Hub", participantes[11].Id, 9.0, 8.5, "Panel centralizado de políticas zero-trust") { Visible = true };
            var tech1_2 = new SustainabilityProject("OceanClean", participantes[12].Id, 8.5, 9.0, "Dron autónomo para limpieza de océanos") { Visible = true };
            var tech1_3 = new AiProject("CodeTranslator", participantes[13].Id, 7.8, 8.2, "Traductor de código entre lenguajes de programación") { Visible = true };
            var tech1_4 = new CybersecurityProject("DeepFake Detector", participantes[14].Id, 9.2, 8.8, "Detección de deepfakes en tiempo real") { Visible = true };
            var techCat1 = new Proyecto[] { tech1_0, tech1_1, tech1_2, tech1_3, tech1_4 };
            foreach (Proyecto p in techCat1)
            {
                p.Categorias.Add(catTech1);
                catTech1.Proyectos.Add(p);
            }

            var tech2_0 = new SustainabilityProject("GreenRoute", participantes[15].Id, 7.5, 8.2, "Rutas de transporte con menor impacto ambiental") { Visible = true };
            var tech2_1 = new AiProject("DataViz Smart", participantes[16].Id, 7.8, 8.0, "Visualización automática de datasets complejos") { Visible = true };
            var tech2_2 = new AiProject("DesignAssist", participantes[17].Id, 8.5, 8.8, "Asistente de diseño UI con sugerencias inteligentes") { Visible = true };
            var tech2_3 = new SustainabilityProject("EcoDashboard", participantes[18].Id, 8.0, 7.5, "Panel de métricas de sostenibilidad corporativa") { Visible = true };
            var tech2_4 = new CybersecurityProject("SecureForms", participantes[19].Id, 7.9, 8.3, "Framework de formularios con validación de seguridad") { Visible = true };
            var techCat2 = new Proyecto[] { tech2_0, tech2_1, tech2_2, tech2_3, tech2_4 };
            foreach (Proyecto p in techCat2)
            {
                p.Categorias.Add(catTech2);
                catTech2.Proyectos.Add(p);
            }

            context.Proyectos.AddRange(techCat1.Concat(techCat2));
            context.SaveChanges();
        }
    }
}
