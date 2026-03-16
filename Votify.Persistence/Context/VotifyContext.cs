using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Votify.Persistence.Entities; 

namespace Votify.Persistence.Context
{
    public class VotifyContext : DbContext
    {
        public VotifyContext(DbContextOptions<VotifyContext> options) : base(options) { }

        // --- Tablas Principales ---
        public DbSet<EventoEntity> Eventos { get; set; }
        public DbSet<VotanteEntity> Votantes { get; set; }
        public DbSet<ProyectoEntity> Proyectos { get; set; }
        public DbSet<CategoriaEntity> Categorias { get; set; }
        public DbSet<PremioEntity> Premios { get; set; }
        public DbSet<VotoEntity> Votos { get; set; }
        public DbSet<CriterioEntity> Criterios { get; set; }

        // --- Tablas con Herencia (Base) ---
        public DbSet<MiembroEntity> Miembros { get; set; }
        public DbSet<VotacionEntity> Votaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Configuración de Herencia TPH para Miembros
            // Esto creará una sola tabla 'Miembros' con una columna 'Discriminator'
            modelBuilder.Entity<MiembroEntity>()
                .HasDiscriminator<string>("TipoMiembro")
                .HasValue<ParticipanteEntity>("Participante")
                .HasValue<JuezEntity>("Juez")
                .HasValue<OrganizadorEntity>("Organizador");

            // 2. Configuración de Herencia TPH para Votaciones
            modelBuilder.Entity<VotacionEntity>()
                .HasDiscriminator<string>("TipoVotacion")
                .HasValue<MulticriterioEntity>("Multicriterio")
                .HasValue<PuntuacionEntity>("Puntuacion")
                .HasValue<PopularEntity>("Popular");

            // 3. Relación Muchos a Muchos: Eventos <-> Miembros
            // Según tu diagrama, un miembro puede estar en muchos eventos y viceversa
            modelBuilder.Entity<EventoEntity>()
                .HasMany(e => e.Miembros)
                .WithMany(m => m.Eventos);

            // 4. Relación Muchos a Muchos: Eventos <-> Votantes
            // Tal como pediste, Votante es independiente y tiene su propia relación
            modelBuilder.Entity<EventoEntity>()
                .HasMany(e => e.Votantes)
                .WithMany(v => v.Eventos);

            // 5. Configuración específica para el Organizador del Evento
            modelBuilder.Entity<EventoEntity>()
                .HasOne(e => e.Organizador)
                .WithMany() // Un organizador puede organizar muchos eventos
                .HasForeignKey(e => e.OrganizadorId)
                .OnDelete(DeleteBehavior.Restrict); // Evita borrar al organizador si hay eventos

            // 6. Configuración de Categoría y Votación
            modelBuilder.Entity<CategoriaEntity>()
                .HasOne(c => c.Votacion)
                .WithOne(v => v.Categoria)
                .HasForeignKey<VotacionEntity>(v => v.CategoriaId);

            // Le decimos a EF Core cómo es la relación 1 a 1
            modelBuilder.Entity<ParticipanteEntity>()
                .HasOne(p => p.Proyecto) // Un participante tiene un proyecto
                .WithOne(pr => pr.Participante) // Un proyecto pertenece a un participante
                .HasForeignKey<ProyectoEntity>(pr => pr.ParticipanteId); // ¡El Proyecto es el que guarda el ID!
        }
    }
}