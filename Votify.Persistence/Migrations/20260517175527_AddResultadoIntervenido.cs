using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Votify.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddResultadoIntervenido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResultadosIntervenidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VotacionId = table.Column<int>(type: "integer", nullable: false),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Posicion = table.Column<int>(type: "integer", nullable: false),
                    PuntajeOriginal = table.Column<double>(type: "double precision", nullable: false),
                    FechaIntervencion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultadosIntervenidos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultadosIntervenidos_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResultadosIntervenidos_Votaciones_VotacionId",
                        column: x => x.VotacionId,
                        principalTable: "Votaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResultadosIntervenidos_ProyectoId",
                table: "ResultadosIntervenidos",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultadosIntervenidos_VotacionId_ProyectoId",
                table: "ResultadosIntervenidos",
                columns: new[] { "VotacionId", "ProyectoId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResultadosIntervenidos");
        }
    }
}
