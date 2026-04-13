using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Votify.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddComentarioToVoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Miembros");

            migrationBuilder.DropColumn(
                name: "Visible",
                table: "Miembros");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
