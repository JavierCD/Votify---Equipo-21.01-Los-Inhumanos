using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Votify.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPermisosVisibilidadYEsPublico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MostrarComentarios",
                table: "Votaciones",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MostrarNombresJueces",
                table: "Votaciones",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MostrarRanking",
                table: "Votaciones",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MostrarResultadosDetallados",
                table: "Votaciones",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EsPublico",
                table: "Eventos",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MostrarComentarios",
                table: "Votaciones");

            migrationBuilder.DropColumn(
                name: "MostrarNombresJueces",
                table: "Votaciones");

            migrationBuilder.DropColumn(
                name: "MostrarRanking",
                table: "Votaciones");

            migrationBuilder.DropColumn(
                name: "MostrarResultadosDetallados",
                table: "Votaciones");

            migrationBuilder.DropColumn(
                name: "EsPublico",
                table: "Eventos");
        }
    }
}
