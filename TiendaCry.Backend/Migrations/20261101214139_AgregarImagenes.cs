using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TiendaCry.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AgregarImagenes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DireccionImagen",
                table: "Promociones");

            migrationBuilder.CreateTable(
                name: "Imagenes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreOriginal = table.Column<string>(type: "text", nullable: false),
                    NombreArchivo = table.Column<string>(type: "text", nullable: false),
                    RutaRelativa = table.Column<string>(type: "text", nullable: false),
                    TipoContenido = table.Column<string>(type: "text", nullable: false),
                    TamanoBytes = table.Column<long>(type: "bigint", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IdPromocion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imagenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Imagenes_Promociones_IdPromocion",
                        column: x => x.IdPromocion,
                        principalTable: "Promociones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Imagenes_IdPromocion",
                table: "Imagenes",
                column: "IdPromocion",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Imagenes");

            migrationBuilder.AddColumn<string>(
                name: "DireccionImagen",
                table: "Promociones",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
