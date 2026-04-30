using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models;
using Votify.Core.Enums;


 

namespace Votify.Persistence.Context
{
    public class VotifyContext : DbContext
    {
        public VotifyContext(DbContextOptions<VotifyContext> options) : base(options) { }

        // --- Tablas Principales ---
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Votante> Votantes { get; set; }
        public DbSet<Proyecto> Proyectos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Premio> Premios { get; set; }
        public DbSet<Voto> Votos { get; set; }
        public DbSet<Criterio> Criterios { get; set; }
        public DbSet<DetalleVoto> DetallesVoto { get; set; }

        // --- Tablas con Herencia (Base) ---
        public DbSet<Miembro> Miembros { get; set; }
        public DbSet<Votacion> Votaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. CONFIGURACIÓN DE MIEMBRO (Familia de Usuarios)
            modelBuilder.Entity<Miembro>(entity =>
            {
                entity.ToTable("Miembros");
                entity.HasKey(m => m.Id);

                entity.Property(m => m.Name).IsRequired().HasMaxLength(100);
                entity.Property(m => m.Email).IsRequired().HasMaxLength(150);
                entity.Property(m => m.Password).IsRequired().HasMaxLength(256);

                entity.HasDiscriminator<string>("TipoDeMiembro")
                      .HasValue<Juez>("Juez")
                      .HasValue<Participante>("Participante")
                      .HasValue<Organizador>("Organizador");

                // ¡AQUÍ BORRAMOS LA RELACIÓN QUE CAUSABA EL ERROR!
            });

            // 2. CONFIGURACIÓN DE PARTICIPANTE
            modelBuilder.Entity<Participante>(entity =>
            {
              
            });

            // 3. CONFIGURACIÓN DE EVENTO
            modelBuilder.Entity<Evento>(entity =>
            {
                entity.ToTable("Eventos");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).IsRequired().HasMaxLength(150);

                entity.Property(e => e.Estado)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(EstadoEvento.Borrador);

                // Relación 1 a N con Organizador
                entity.HasOne(e => e.Organizador)
                      .WithMany()
                      .HasForeignKey(e => e.OrganizadorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasDiscriminator<string>("Discriminador")
                .HasValue<HackathonEvent>("Hackathon")
                .HasValue<InnovationFairEvent>("FeriaInnovacion")
                .HasValue<ESportsEvent>("ESport");

                // ---> ¡NUEVO! Separamos las listas para que EF no se vuelva loco <---

                // Relación N a N con Jurado (Jueces)
                entity.HasMany(e => e.Jurado)
                      .WithMany()
                      .UsingEntity(j => j.ToTable("EventosJurado"));

                // Relación N a N con Participantes
                entity.HasMany(e => e.Participantes)
                      .WithMany()
                      .UsingEntity(j => j.ToTable("EventosParticipantes"));
            });

            // 4. CONFIGURACIÓN DE VOTACIÓN (Familia)
            modelBuilder.Entity<Votacion>(entity =>
            {
                entity.ToTable("Votaciones");
                entity.HasKey(v => v.Id);

                entity.Property(v => v.Estado)
                      .IsRequired()
                      .HasMaxLength(50)
                      .HasDefaultValue("Cerrada");

                entity.HasDiscriminator<string>("TipoVotacion")
                      .HasValue<Multicriterio>("Multicriterio")
                      .HasValue<Popular>("Popular")
                      .HasValue<Puntuacion>("Puntuacion");

                entity.HasOne(v => v.Categoria)
                      .WithOne(c => c.Votacion)
                      .HasForeignKey<Votacion>(v => v.CategoriaId)
                      .OnDelete(DeleteBehavior.Cascade);


            });

            // 4.1. Propiedades específicas de Multicriterio
            modelBuilder.Entity<Multicriterio>(entity =>
            {
                // No hace falta poner ToTable porque hereda de Votacion
                entity.Property(m => m.UsaPesos).HasDefaultValue(true);

                // CONFIGURACIÓN DE LA RELACIÓN CON CRITERIOS
                entity.HasMany(m => m.Criterios)
                      .WithOne(c => c.Multicriterio) // Usamos la propiedad de navegación explícita
                      .HasForeignKey(c => c.MulticriterioId) // Clave foránea explícita
                      .OnDelete(DeleteBehavior.Cascade); // Si borras la votación, se borran sus criterios
            });

            // 4.2. Propiedades específicas de Puntuacion
            modelBuilder.Entity<Puntuacion>(entity =>
            {
                entity.Property(p => p.ValorMax).HasDefaultValue(5);
            });

            // 5. CONFIGURACIÓN DE CATEGORÍA
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.ToTable("Categorias");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);

                entity.HasOne(c => c.Evento)
                      .WithMany(e => e.CategoriasEvento) // Enganchamos con la lista de Evento
                      .HasForeignKey(c => c.EventoId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Votacion)
                      .WithOne(v => v.Categoria)
                      .HasForeignKey<Votacion>(v => v.CategoriaId)
                      .IsRequired(false);

                entity.HasMany(c => c.Premios)
                      .WithOne(p => p.Categoria)
                      .HasForeignKey(p => p.CategoriaId)
                      .IsRequired(false);
            });

            // 6. CONFIGURACIÓN DE CRITERIO
            modelBuilder.Entity<Criterio>(entity =>
            {
                entity.ToTable("Criterios");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);

                entity.HasOne(c => c.Multicriterio)
                      .WithMany(m => ((Multicriterio)m).Criterios) // Enganchamos con Multicriterio
                      .HasForeignKey(c => c.MulticriterioId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 7. CONFIGURACIÓN DE PREMIO
            // 7. CONFIGURACIÓN DE PREMIO
            modelBuilder.Entity<Premio>(entity =>
            {
                entity.ToTable("Premios");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);

                // ---> NUEVO: Configuración de la regla de empates <---
                entity.Property(p => p.PermiteEmpate).HasDefaultValue(false);

                entity.HasOne(p => p.Categoria)
                      .WithMany(c => c.Premios)
                      .HasForeignKey(p => p.CategoriaId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 8. CONFIGURACIÓN DE PROYECTO
            modelBuilder.Entity<Proyecto>(entity =>
            {
                entity.ToTable("Proyectos");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
                //entity.Property(p => p.Description).HasMaxLength(500);

                entity.HasOne(p => p.Participante)
                      .WithMany(p => p.Proyectos)
                      .HasForeignKey(p => p.ParticipanteId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(p => p.Categorias)
                      .WithMany(c => c.Proyectos)
                      .UsingEntity(j => j.ToTable("ProyectosCategorias"));

                entity.HasDiscriminator<string>("TipoProyecto")
                .HasValue<AiProject>("AI")
                .HasValue<SustainabilityProject>("Sostenibilidad")
                .HasValue<CybersecurityProject>("Ciberseguridad");
            });

            // 9. CONFIGURACIÓN DE VOTANTE
            modelBuilder.Entity<Votante>(entity =>
            {
                entity.ToTable("Votantes");
                entity.HasKey(v => v.Id);
                entity.Property(v => v.Email).IsRequired().HasMaxLength(150);
                entity.Property(v => v.Anonimo).HasDefaultValue(false);

                entity.HasMany(v => v.Eventos)
                      .WithMany(e => e.Votantes)
                      .UsingEntity(j => j.ToTable("EventosVotantes"));

                // ¡AQUÍ ESTÁ LA MAGIA! Le decimos explícitamente cómo se relaciona con VotoPublico
                entity.HasMany(v => v.Votos)
                      .WithOne(vp => vp.Votante)
                      .HasForeignKey(vp => vp.VotanteId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 10. CONFIGURACIÓN DE VOTO
            modelBuilder.Entity<Voto>(entity =>
            {
                entity.ToTable("Votos");
                entity.HasKey(v => v.Id);
                entity.Property(v => v.HashAnonimo).HasMaxLength(256);

                entity.HasDiscriminator<string>("TipoVoto")
                .HasValue<VotoExperto>("Experto")
                .HasValue<VotoPublico>("Publico")
                .HasValue<VotoSponsor>("Sponsor");

                entity.HasOne(v => v.Votacion)
                      .WithMany(v => v.Votos)
                      .HasForeignKey(v => v.VotacionId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(v => v.Proyecto)
                      .WithMany(p => p.Votos)
                      .HasForeignKey(v => v.ProyectoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 11. CONFIGURACIÓN EXCLUSIVA DE VOTO EXPERTO
            modelBuilder.Entity<VotoExperto>(entity =>
            {
                entity.HasOne(ve => ve.Juez)
                      .WithMany() // Un juez no tiene una lista de VotoExperto en su clase de momento
                      .HasForeignKey(ve => ve.JuezId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 12. CONFIGURACIÓN DE DETALLE VOTO
            modelBuilder.Entity<DetalleVoto>(entity =>
            {
                entity.ToTable("DetallesVoto"); // Obligamos a que la tabla se llame así
                entity.HasKey(d => d.Id);

                // Relación con Voto
                entity.HasOne(d => d.Voto)
                      .WithMany(v => v.Detalles)
                      .HasForeignKey(d => d.VotoId)
                      .OnDelete(DeleteBehavior.Cascade); // Si borramos el voto, borramos sus detalles

                // Relación con Proyecto
                entity.HasOne(d => d.Proyecto)
                      .WithMany() // Suponiendo que Proyecto no tiene List<DetalleVoto>
                      .HasForeignKey(d => d.ProyectoId)
                      .OnDelete(DeleteBehavior.Restrict); // Evita borrado múltiple en cascada

                // Relación con Criterio (opcional)
                entity.HasOne(d => d.Criterio)
                      .WithMany()
                      .HasForeignKey(d => d.CriterioId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}