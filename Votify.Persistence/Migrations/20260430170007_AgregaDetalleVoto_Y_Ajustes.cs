using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Votify.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregaDetalleVoto_Y_Ajustes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votos_Miembros_JuezId",
                table: "Votos");

            migrationBuilder.DropForeignKey(
                name: "FK_Votos_Votantes_VotanteId",
                table: "Votos");

            migrationBuilder.DropColumn(
                name: "VotacionId",
                table: "Criterios");

            migrationBuilder.CreateTable(
                name: "DetallesVoto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VotoId = table.Column<int>(type: "integer", nullable: false),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    CriterioId = table.Column<int>(type: "integer", nullable: true),
                    Puntuacion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesVoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesVoto_Criterios_CriterioId",
                        column: x => x.CriterioId,
                        principalTable: "Criterios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetallesVoto_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetallesVoto_Votos_VotoId",
                        column: x => x.VotoId,
                        principalTable: "Votos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVoto_CriterioId",
                table: "DetallesVoto",
                column: "CriterioId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVoto_ProyectoId",
                table: "DetallesVoto",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVoto_VotoId",
                table: "DetallesVoto",
                column: "VotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Votos_Miembros_JuezId",
                table: "Votos",
                column: "JuezId",
                principalTable: "Miembros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Votos_Votantes_VotanteId",
                table: "Votos",
                column: "VotanteId",
                principalTable: "Votantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votos_Miembros_JuezId",
                table: "Votos");

            migrationBuilder.DropForeignKey(
                name: "FK_Votos_Votantes_VotanteId",
                table: "Votos");

            migrationBuilder.DropTable(
                name: "DetallesVoto");

            migrationBuilder.AddColumn<int>(
                name: "VotacionId",
                table: "Criterios",
                type: "integer",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Votos_Miembros_JuezId",
                table: "Votos",
                column: "JuezId",
                principalTable: "Miembros",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Votos_Votantes_VotanteId",
                table: "Votos",
                column: "VotanteId",
                principalTable: "Votantes",
                principalColumn: "Id");
        }
    }
}
