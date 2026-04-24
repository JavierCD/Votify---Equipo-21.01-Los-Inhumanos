using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Votify.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPermiteAutoVotoToPopular : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rol",
                table: "Votantes");

            migrationBuilder.AddColumn<string>(
                name: "Comentario",
                table: "Votos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EnviarNotificacionApertura",
                table: "Votaciones",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotificacionAperturaEnviada",
                table: "Votaciones",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PermiteAutoVoto",
                table: "Votaciones",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "QuiereRecibirNotificaciones",
                table: "Miembros",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Notificacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MiembroId = table.Column<int>(type: "integer", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Mensaje = table.Column<string>(type: "text", nullable: false),
                    UrlAccion = table.Column<string>(type: "text", nullable: false),
                    Leida = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notificacion_Miembros_MiembroId",
                        column: x => x.MiembroId,
                        principalTable: "Miembros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notificacion_MiembroId",
                table: "Notificacion",
                column: "MiembroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notificacion");

            migrationBuilder.DropColumn(
                name: "Comentario",
                table: "Votos");

            migrationBuilder.DropColumn(
                name: "EnviarNotificacionApertura",
                table: "Votaciones");

            migrationBuilder.DropColumn(
                name: "NotificacionAperturaEnviada",
                table: "Votaciones");

            migrationBuilder.DropColumn(
                name: "PermiteAutoVoto",
                table: "Votaciones");

            migrationBuilder.DropColumn(
                name: "QuiereRecibirNotificaciones",
                table: "Miembros");

            migrationBuilder.AddColumn<string>(
                name: "Rol",
                table: "Votantes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
