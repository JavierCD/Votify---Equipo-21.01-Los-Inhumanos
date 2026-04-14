using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Votify.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ResultadosVotacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votos_Votantes_votanteId",
                table: "Votos");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Miembros");

            migrationBuilder.DropColumn(
                name: "Visible",
                table: "Miembros");

            migrationBuilder.RenameColumn(
                name: "votanteId",
                table: "Votos",
                newName: "VotanteId");

            migrationBuilder.RenameIndex(
                name: "IX_Votos_votanteId",
                table: "Votos",
                newName: "IX_Votos_VotanteId");

            migrationBuilder.AddColumn<bool>(
                name: "EstaCerrada",
                table: "Votaciones",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ResultadosPublicados",
                table: "Votaciones",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ColorFondo",
                table: "Miembros",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstitucionEducativa",
                table: "Miembros",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Intereses",
                table: "Miembros",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrlFoto",
                table: "Miembros",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Votos_Votantes_VotanteId",
                table: "Votos",
                column: "VotanteId",
                principalTable: "Votantes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votos_Votantes_VotanteId",
                table: "Votos");

            migrationBuilder.DropColumn(
                name: "EstaCerrada",
                table: "Votaciones");

            migrationBuilder.DropColumn(
                name: "ResultadosPublicados",
                table: "Votaciones");

            migrationBuilder.DropColumn(
                name: "ColorFondo",
                table: "Miembros");

            migrationBuilder.DropColumn(
                name: "InstitucionEducativa",
                table: "Miembros");

            migrationBuilder.DropColumn(
                name: "Intereses",
                table: "Miembros");

            migrationBuilder.DropColumn(
                name: "UrlFoto",
                table: "Miembros");

            migrationBuilder.RenameColumn(
                name: "VotanteId",
                table: "Votos",
                newName: "votanteId");

            migrationBuilder.RenameIndex(
                name: "IX_Votos_VotanteId",
                table: "Votos",
                newName: "IX_Votos_votanteId");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Miembros",
                type: "text",
                nullable: true,
                defaultValue: "Pendiente");

            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                table: "Miembros",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Votos_Votantes_votanteId",
                table: "Votos",
                column: "votanteId",
                principalTable: "Votantes",
                principalColumn: "Id");
        }
    }
}
