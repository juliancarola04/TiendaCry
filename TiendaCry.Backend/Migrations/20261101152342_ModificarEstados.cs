using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TiendaCry.Backend.Migrations
{
    /// <inheritdoc />
    public partial class ModificarEstados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Socios_EstadosSocio_IdEstadoSocio",
                table: "Socios");

            migrationBuilder.DropTable(
                name: "EstadosSocio");

            migrationBuilder.DropIndex(
                name: "IX_Socios_IdEstadoSocio",
                table: "Socios");

            migrationBuilder.DropColumn(
                name: "IdEstadoSocio",
                table: "Socios");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdEstadoSocio",
                table: "Socios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EstadosSocio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosSocio", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Socios_IdEstadoSocio",
                table: "Socios",
                column: "IdEstadoSocio");

            migrationBuilder.AddForeignKey(
                name: "FK_Socios_EstadosSocio_IdEstadoSocio",
                table: "Socios",
                column: "IdEstadoSocio",
                principalTable: "EstadosSocio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
