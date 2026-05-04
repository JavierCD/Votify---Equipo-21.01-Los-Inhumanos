using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Votify.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregaCamposProyecto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NombresEquipo",
                table: "Proyectos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrlMaterialesExternos",
                table: "Proyectos",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NombresEquipo",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "UrlMaterialesExternos",
                table: "Proyectos");
        }
    }
}
