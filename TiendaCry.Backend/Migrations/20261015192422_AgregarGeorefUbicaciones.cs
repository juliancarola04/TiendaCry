using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiendaCry.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AgregarGeorefUbicaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GeorefId",
                table: "Provincias",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GeorefId",
                table: "Ciudades",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeorefId",
                table: "Provincias");

            migrationBuilder.DropColumn(
                name: "GeorefId",
                table: "Ciudades");
        }
    }
}
