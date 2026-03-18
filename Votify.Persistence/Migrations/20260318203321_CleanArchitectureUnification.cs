using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Votify.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CleanArchitectureUnification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Criterios_Votaciones_VotacionId",
                table: "Criterios");

            migrationBuilder.DropForeignKey(
                name: "FK_Votos_Miembros_JuezId",
                table: "Votos");

            migrationBuilder.DropForeignKey(
                name: "FK_Votos_Votantes_VotanteId",
                table: "Votos");

            migrationBuilder.DropTable(
                name: "CategoriaEntityProyectoEntity");

            migrationBuilder.DropTable(
                name: "EventoEntityMiembroEntity");

            migrationBuilder.DropTable(
                name: "EventoEntityVotanteEntity");

            migrationBuilder.DropIndex(
                name: "IX_Criterios_VotacionId",
                table: "Criterios");

            migrationBuilder.DropColumn(
                name: "ProyectoId",
                table: "Miembros");

            migrationBuilder.DropColumn(
                name: "VotacionId",
                table: "Criterios");

            migrationBuilder.RenameColumn(
                name: "TipoMiembro",
                table: "Miembros",
                newName: "TipoDeMiembro");

            migrationBuilder.AlterColumn<string>(
                name: "HashAnonimo",
                table: "Votos",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProyectoId",
                table: "Votos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Votantes",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "Anonimo",
                table: "Votantes",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "ValorMax",
                table: "Votaciones",
                type: "integer",
                nullable: true,
                defaultValue: 5,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "UsaPesos",
                table: "Votaciones",
                type: "boolean",
                nullable: true,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TipoVotacion",
                table: "Votaciones",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(21)",
                oldMaxLength: 21);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Votaciones",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Cerrada",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Proyectos",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Premios",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Miembros",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Miembros",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Miembros",
                type: "text",
                nullable: true,
                defaultValue: "Pendiente",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Miembros",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Eventos",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Eventos",
                type: "text",
                nullable: false,
                defaultValue: "Borrador",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Criterios",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categorias",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

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

            migrationBuilder.CreateIndex(
                name: "IX_Votos_ProyectoId",
                table: "Votos",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_Criterios_MulticriterioId",
                table: "Criterios",
                column: "MulticriterioId");

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
                name: "IX_ProyectosCategorias_ProyectosId",
                table: "ProyectosCategorias",
                column: "ProyectosId");

            migrationBuilder.AddForeignKey(
                name: "FK_Criterios_Votaciones_MulticriterioId",
                table: "Criterios",
                column: "MulticriterioId",
                principalTable: "Votaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Votos_Miembros_JuezId",
                table: "Votos",
                column: "JuezId",
                principalTable: "Miembros",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Votos_Proyectos_ProyectoId",
                table: "Votos",
                column: "ProyectoId",
                principalTable: "Proyectos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Votos_Votantes_VotanteId",
                table: "Votos",
                column: "VotanteId",
                principalTable: "Votantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Criterios_Votaciones_MulticriterioId",
                table: "Criterios");

            migrationBuilder.DropForeignKey(
                name: "FK_Votos_Miembros_JuezId",
                table: "Votos");

            migrationBuilder.DropForeignKey(
                name: "FK_Votos_Proyectos_ProyectoId",
                table: "Votos");

            migrationBuilder.DropForeignKey(
                name: "FK_Votos_Votantes_VotanteId",
                table: "Votos");

            migrationBuilder.DropTable(
                name: "EventosJurado");

            migrationBuilder.DropTable(
                name: "EventosParticipantes");

            migrationBuilder.DropTable(
                name: "EventosVotantes");

            migrationBuilder.DropTable(
                name: "ProyectosCategorias");

            migrationBuilder.DropIndex(
                name: "IX_Votos_ProyectoId",
                table: "Votos");

            migrationBuilder.DropIndex(
                name: "IX_Criterios_MulticriterioId",
                table: "Criterios");

            migrationBuilder.DropColumn(
                name: "ProyectoId",
                table: "Votos");

            migrationBuilder.RenameColumn(
                name: "TipoDeMiembro",
                table: "Miembros",
                newName: "TipoMiembro");

            migrationBuilder.AlterColumn<string>(
                name: "HashAnonimo",
                table: "Votos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Votantes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<bool>(
                name: "Anonimo",
                table: "Votantes",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "ValorMax",
                table: "Votaciones",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true,
                oldDefaultValue: 5);

            migrationBuilder.AlterColumn<bool>(
                name: "UsaPesos",
                table: "Votaciones",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true,
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "TipoVotacion",
                table: "Votaciones",
                type: "character varying(21)",
                maxLength: 21,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(13)",
                oldMaxLength: 13);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Votaciones",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Cerrada");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Proyectos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Premios",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Miembros",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Miembros",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Miembros",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldDefaultValue: "Pendiente");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Miembros",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AddColumn<int>(
                name: "ProyectoId",
                table: "Miembros",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Eventos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Eventos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "Borrador");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Criterios",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "VotacionId",
                table: "Criterios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categorias",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateTable(
                name: "CategoriaEntityProyectoEntity",
                columns: table => new
                {
                    CategoriasId = table.Column<int>(type: "integer", nullable: false),
                    ProyectosId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaEntityProyectoEntity", x => new { x.CategoriasId, x.ProyectosId });
                    table.ForeignKey(
                        name: "FK_CategoriaEntityProyectoEntity_Categorias_CategoriasId",
                        column: x => x.CategoriasId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoriaEntityProyectoEntity_Proyectos_ProyectosId",
                        column: x => x.ProyectosId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventoEntityMiembroEntity",
                columns: table => new
                {
                    EventosId = table.Column<int>(type: "integer", nullable: false),
                    MiembrosId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoEntityMiembroEntity", x => new { x.EventosId, x.MiembrosId });
                    table.ForeignKey(
                        name: "FK_EventoEntityMiembroEntity_Eventos_EventosId",
                        column: x => x.EventosId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventoEntityMiembroEntity_Miembros_MiembrosId",
                        column: x => x.MiembrosId,
                        principalTable: "Miembros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventoEntityVotanteEntity",
                columns: table => new
                {
                    EventosId = table.Column<int>(type: "integer", nullable: false),
                    VotantesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoEntityVotanteEntity", x => new { x.EventosId, x.VotantesId });
                    table.ForeignKey(
                        name: "FK_EventoEntityVotanteEntity_Eventos_EventosId",
                        column: x => x.EventosId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventoEntityVotanteEntity_Votantes_VotantesId",
                        column: x => x.VotantesId,
                        principalTable: "Votantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Criterios_VotacionId",
                table: "Criterios",
                column: "VotacionId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriaEntityProyectoEntity_ProyectosId",
                table: "CategoriaEntityProyectoEntity",
                column: "ProyectosId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoEntityMiembroEntity_MiembrosId",
                table: "EventoEntityMiembroEntity",
                column: "MiembrosId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoEntityVotanteEntity_VotantesId",
                table: "EventoEntityVotanteEntity",
                column: "VotantesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Criterios_Votaciones_VotacionId",
                table: "Criterios",
                column: "VotacionId",
                principalTable: "Votaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
