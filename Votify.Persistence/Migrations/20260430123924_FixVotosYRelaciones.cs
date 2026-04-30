using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Votify.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixVotosYRelaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JuezId",
                table: "Votos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SponsorId",
                table: "Votos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VotacionId",
                table: "Miembros",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votos_JuezId",
                table: "Votos",
                column: "JuezId");

            migrationBuilder.CreateIndex(
                name: "IX_Miembros_VotacionId",
                table: "Miembros",
                column: "VotacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Miembros_Votaciones_VotacionId",
                table: "Miembros",
                column: "VotacionId",
                principalTable: "Votaciones",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Votos_Miembros_JuezId",
                table: "Votos",
                column: "JuezId",
                principalTable: "Miembros",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Miembros_Votaciones_VotacionId",
                table: "Miembros");

            migrationBuilder.DropForeignKey(
                name: "FK_Votos_Miembros_JuezId",
                table: "Votos");

            migrationBuilder.DropIndex(
                name: "IX_Votos_JuezId",
                table: "Votos");

            migrationBuilder.DropIndex(
                name: "IX_Miembros_VotacionId",
                table: "Miembros");

            migrationBuilder.DropColumn(
                name: "JuezId",
                table: "Votos");

            migrationBuilder.DropColumn(
                name: "SponsorId",
                table: "Votos");

            migrationBuilder.DropColumn(
                name: "VotacionId",
                table: "Miembros");
        }
    }
}
