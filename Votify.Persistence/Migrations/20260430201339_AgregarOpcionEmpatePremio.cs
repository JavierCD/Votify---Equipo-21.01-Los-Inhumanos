using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Votify.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarOpcionEmpatePremio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PermiteEmpate",
                table: "Premios",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PermiteEmpate",
                table: "Premios");
        }
    }
}
