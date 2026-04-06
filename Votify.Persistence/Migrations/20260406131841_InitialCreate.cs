using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Votify.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Miembros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Password = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TipoDeMiembro = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Visible = table.Column<bool>(type: "boolean", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true, defaultValue: "Pendiente")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Miembros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Votantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Anonimo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Rol = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votantes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Eventos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CodigoAcceso = table.Column<string>(type: "text", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false, defaultValue: "Borrador"),
                    OrganizadorId = table.Column<int>(type: "integer", nullable: false),
                    Discriminador = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Eventos_Miembros_OrganizadorId",
                        column: x => x.OrganizadorId,
                        principalTable: "Miembros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Proyectos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    ParticipanteId = table.Column<int>(type: "integer", nullable: false),
                    CriterioA = table.Column<double>(type: "double precision", nullable: false),
                    CriterioB = table.Column<double>(type: "double precision", nullable: false),
                    TipoProyecto = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proyectos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Proyectos_Miembros_ParticipanteId",
                        column: x => x.ParticipanteId,
                        principalTable: "Miembros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    EventoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categorias_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventosJurado",
                columns: table => new
                {
                    EventoId = table.Column<int>(type: "integer", nullable: false),
                    JuradoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventosJurado", x => new { x.EventoId, x.JuradoId });
                    table.ForeignKey(
                        name: "FK_EventosJurado_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventosJurado_Miembros_JuradoId",
                        column: x => x.JuradoId,
                        principalTable: "Miembros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventosParticipantes",
                columns: table => new
                {
                    EventoId = table.Column<int>(type: "integer", nullable: false),
                    ParticipantesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventosParticipantes", x => new { x.EventoId, x.ParticipantesId });
                    table.ForeignKey(
                        name: "FK_EventosParticipantes_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventosParticipantes_Miembros_ParticipantesId",
                        column: x => x.ParticipantesId,
                        principalTable: "Miembros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventosVotantes",
                columns: table => new
                {
                    EventosId = table.Column<int>(type: "integer", nullable: false),
                    VotantesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventosVotantes", x => new { x.EventosId, x.VotantesId });
                    table.ForeignKey(
                        name: "FK_EventosVotantes_Eventos_EventosId",
                        column: x => x.EventosId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventosVotantes_Votantes_VotantesId",
                        column: x => x.VotantesId,
                        principalTable: "Votantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Premios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Posicion = table.Column<int>(type: "integer", nullable: false),
                    CategoriaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Premios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Premios_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProyectosCategorias",
                columns: table => new
                {
                    CategoriasId = table.Column<int>(type: "integer", nullable: false),
                    ProyectosId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectosCategorias", x => new { x.CategoriasId, x.ProyectosId });
                    table.ForeignKey(
                        name: "FK_ProyectosCategorias_Categorias_CategoriasId",
                        column: x => x.CategoriasId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProyectosCategorias_Proyectos_ProyectosId",
                        column: x => x.ProyectosId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Votaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventoId = table.Column<int>(type: "integer", nullable: false),
                    FechaApertura = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Estado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Cerrada"),
                    CategoriaId = table.Column<int>(type: "integer", nullable: false),
                    TipoVotacion = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    UsaPesos = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    MaxSelection = table.Column<int>(type: "integer", nullable: true),
                    ValorMax = table.Column<int>(type: "integer", nullable: true, defaultValue: 5)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votaciones_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Criterios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Peso = table.Column<float>(type: "real", nullable: false),
                    MulticriterioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Criterios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Criterios_Votaciones_MulticriterioId",
                        column: x => x.MulticriterioId,
                        principalTable: "Votaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Votos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Anonimo = table.Column<bool>(type: "boolean", nullable: false),
                    HashAnonimo = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    PuntuacionBase = table.Column<double>(type: "double precision", nullable: false),
                    votanteId = table.Column<int>(type: "integer", nullable: true),
                    VotacionId = table.Column<int>(type: "integer", nullable: false),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    TipoVoto = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votos_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Votos_Votaciones_VotacionId",
                        column: x => x.VotacionId,
                        principalTable: "Votaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Votos_Votantes_votanteId",
                        column: x => x.votanteId,
                        principalTable: "Votantes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_EventoId",
                table: "Categorias",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_Criterios_MulticriterioId",
                table: "Criterios",
                column: "MulticriterioId");

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_OrganizadorId",
                table: "Eventos",
                column: "OrganizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_EventosJurado_JuradoId",
                table: "EventosJurado",
                column: "JuradoId");

            migrationBuilder.CreateIndex(
                name: "IX_EventosParticipantes_ParticipantesId",
                table: "EventosParticipantes",
                column: "ParticipantesId");

            migrationBuilder.CreateIndex(
                name: "IX_EventosVotantes_VotantesId",
                table: "EventosVotantes",
                column: "VotantesId");

            migrationBuilder.CreateIndex(
                name: "IX_Premios_CategoriaId",
                table: "Premios",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Proyectos_ParticipanteId",
                table: "Proyectos",
                column: "ParticipanteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProyectosCategorias_ProyectosId",
                table: "ProyectosCategorias",
                column: "ProyectosId");

            migrationBuilder.CreateIndex(
                name: "IX_Votaciones_CategoriaId",
                table: "Votaciones",
                column: "CategoriaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votos_ProyectoId",
                table: "Votos",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_Votos_VotacionId",
                table: "Votos",
                column: "VotacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Votos_votanteId",
                table: "Votos",
                column: "votanteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Criterios");

            migrationBuilder.DropTable(
                name: "EventosJurado");

            migrationBuilder.DropTable(
                name: "EventosParticipantes");

            migrationBuilder.DropTable(
                name: "EventosVotantes");

            migrationBuilder.DropTable(
                name: "Premios");

            migrationBuilder.DropTable(
                name: "ProyectosCategorias");

            migrationBuilder.DropTable(
                name: "Votos");

            migrationBuilder.DropTable(
                name: "Proyectos");

            migrationBuilder.DropTable(
                name: "Votaciones");

            migrationBuilder.DropTable(
                name: "Votantes");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Eventos");

            migrationBuilder.DropTable(
                name: "Miembros");
        }
    }
}
