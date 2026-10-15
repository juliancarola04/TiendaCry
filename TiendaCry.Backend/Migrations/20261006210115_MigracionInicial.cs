using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TiendaCry.Backend.Migrations
{
    /// <inheritdoc />
    public partial class MigracionInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosCupon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosCupon", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosMembresia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosMembresia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosPagoMembresia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosPagoMembresia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosPromocion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosPromocion", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "EstadosTienda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosTienda", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosUsuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosUsuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosUsuarioTienda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosUsuarioTienda", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Provincias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolesTienda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesTienda", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposTokenEmail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposTokenEmail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomUsuario = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Contrasena = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    EmailVerificado = table.Column<bool>(type: "boolean", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false),
                    Apellido = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false),
                    EsAdministradorTiendaCry = table.Column<bool>(type: "boolean", nullable: false),
                    DebeCambiarContrasena = table.Column<bool>(type: "boolean", nullable: false),
                    IdEstadoUsuario = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_EstadosUsuario_IdEstadoUsuario",
                        column: x => x.IdEstadoUsuario,
                        principalTable: "EstadosUsuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ciudades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    ProvinciaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ciudades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ciudades_Provincias_ProvinciaId",
                        column: x => x.ProvinciaId,
                        principalTable: "Provincias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TokensEmail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmailNuevo = table.Column<string>(type: "text", nullable: true),
                    Usado = table.Column<bool>(type: "boolean", nullable: false),
                    IdTipoTokenEmail = table.Column<int>(type: "integer", nullable: false),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokensEmail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TokensEmail_TiposTokenEmail_IdTipoTokenEmail",
                        column: x => x.IdTipoTokenEmail,
                        principalTable: "TiposTokenEmail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokensEmail_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Socios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NumSocio = table.Column<string>(type: "text", nullable: false),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false),
                    IdEstadoSocio = table.Column<int>(type: "integer", nullable: false),
                    IdCiudad = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Socios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Socios_Ciudades_IdCiudad",
                        column: x => x.IdCiudad,
                        principalTable: "Ciudades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Socios_EstadosSocio_IdEstadoSocio",
                        column: x => x.IdEstadoSocio,
                        principalTable: "EstadosSocio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Socios_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tiendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Calle = table.Column<string>(type: "text", nullable: false),
                    NumCalle = table.Column<string>(type: "text", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaRevision = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MotivoRechazo = table.Column<string>(type: "text", nullable: true),
                    IdCiudad = table.Column<int>(type: "integer", nullable: false),
                    IdEstadoTienda = table.Column<int>(type: "integer", nullable: false),
                    IdCategoria = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tiendas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tiendas_Categorias_IdCategoria",
                        column: x => x.IdCategoria,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tiendas_Ciudades_IdCiudad",
                        column: x => x.IdCiudad,
                        principalTable: "Ciudades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tiendas_EstadosTienda_IdEstadoTienda",
                        column: x => x.IdEstadoTienda,
                        principalTable: "EstadosTienda",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Membresias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MontoMensualCupones = table.Column<int>(type: "integer", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IdSocio = table.Column<int>(type: "integer", nullable: false),
                    IdEstadoMembresia = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Membresias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Membresias_EstadosMembresia_IdEstadoMembresia",
                        column: x => x.IdEstadoMembresia,
                        principalTable: "EstadosMembresia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Membresias_Socios_IdSocio",
                        column: x => x.IdSocio,
                        principalTable: "Socios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Promociones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    DireccionImagen = table.Column<string>(type: "text", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaRevision = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MotivoRechazo = table.Column<string>(type: "text", nullable: true),
                    IdTienda = table.Column<int>(type: "integer", nullable: false),
                    IdEstadoPromocion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promociones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Promociones_EstadosPromocion_IdEstadoPromocion",
                        column: x => x.IdEstadoPromocion,
                        principalTable: "EstadosPromocion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Promociones_Tiendas_IdTienda",
                        column: x => x.IdTienda,
                        principalTable: "Tiendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosTienda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FechaAlta = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false),
                    IdTienda = table.Column<int>(type: "integer", nullable: false),
                    IdRolTienda = table.Column<int>(type: "integer", nullable: false),
                    IdEstadoUsuarioTienda = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosTienda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuariosTienda_EstadosUsuarioTienda_IdEstadoUsuarioTienda",
                        column: x => x.IdEstadoUsuarioTienda,
                        principalTable: "EstadosUsuarioTienda",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuariosTienda_RolesTienda_IdRolTienda",
                        column: x => x.IdRolTienda,
                        principalTable: "RolesTienda",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuariosTienda_Tiendas_IdTienda",
                        column: x => x.IdTienda,
                        principalTable: "Tiendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuariosTienda_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cupones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdTiendaNube = table.Column<string>(type: "text", nullable: false),
                    Monto = table.Column<int>(type: "integer", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaUso = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IdEstadoCupon = table.Column<int>(type: "integer", nullable: false),
                    IdMembresia = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cupones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cupones_EstadosCupon_IdEstadoCupon",
                        column: x => x.IdEstadoCupon,
                        principalTable: "EstadosCupon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cupones_Membresias_IdMembresia",
                        column: x => x.IdMembresia,
                        principalTable: "Membresias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PagosMembresiaMercadoPago",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Monto = table.Column<int>(type: "integer", nullable: false),
                    Moneda = table.Column<string>(type: "text", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IdPagoMP = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IdPreferenciaMP = table.Column<string>(type: "text", nullable: false),
                    ExternalReferenceMP = table.Column<string>(type: "text", nullable: false),
                    IdEstadoPagoMembresia = table.Column<int>(type: "integer", nullable: false),
                    IdMembresia = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosMembresiaMercadoPago", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagosMembresiaMercadoPago_EstadosPagoMembresia_IdEstadoPago~",
                        column: x => x.IdEstadoPagoMembresia,
                        principalTable: "EstadosPagoMembresia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PagosMembresiaMercadoPago_Membresias_IdMembresia",
                        column: x => x.IdMembresia,
                        principalTable: "Membresias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TokensQR",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "text", nullable: false),
                    FechaGeneracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IdMembresia = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokensQR", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TokensQR_Membresias_IdMembresia",
                        column: x => x.IdMembresia,
                        principalTable: "Membresias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsosPromocion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FechaHoraUso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IdMembresia = table.Column<int>(type: "integer", nullable: false),
                    IdPromocion = table.Column<int>(type: "integer", nullable: false),
                    IdUsuarioTienda = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsosPromocion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsosPromocion_Membresias_IdMembresia",
                        column: x => x.IdMembresia,
                        principalTable: "Membresias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsosPromocion_Promociones_IdPromocion",
                        column: x => x.IdPromocion,
                        principalTable: "Promociones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsosPromocion_UsuariosTienda_IdUsuarioTienda",
                        column: x => x.IdUsuarioTienda,
                        principalTable: "UsuariosTienda",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ciudades_ProvinciaId",
                table: "Ciudades",
                column: "ProvinciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Cupones_IdEstadoCupon",
                table: "Cupones",
                column: "IdEstadoCupon");

            migrationBuilder.CreateIndex(
                name: "IX_Cupones_IdMembresia",
                table: "Cupones",
                column: "IdMembresia");

            migrationBuilder.CreateIndex(
                name: "IX_EstadosUsuario_Nombre",
                table: "EstadosUsuario",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Membresias_IdEstadoMembresia",
                table: "Membresias",
                column: "IdEstadoMembresia");

            migrationBuilder.CreateIndex(
                name: "IX_Membresias_IdSocio",
                table: "Membresias",
                column: "IdSocio");

            migrationBuilder.CreateIndex(
                name: "IX_PagosMembresiaMercadoPago_IdEstadoPagoMembresia",
                table: "PagosMembresiaMercadoPago",
                column: "IdEstadoPagoMembresia");

            migrationBuilder.CreateIndex(
                name: "IX_PagosMembresiaMercadoPago_IdMembresia",
                table: "PagosMembresiaMercadoPago",
                column: "IdMembresia");

            migrationBuilder.CreateIndex(
                name: "IX_PagosMembresiaMercadoPago_IdPagoMP",
                table: "PagosMembresiaMercadoPago",
                column: "IdPagoMP",
                unique: true,
                filter: "\"IdPagoMP\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Promociones_IdEstadoPromocion",
                table: "Promociones",
                column: "IdEstadoPromocion");

            migrationBuilder.CreateIndex(
                name: "IX_Promociones_IdTienda",
                table: "Promociones",
                column: "IdTienda");

            migrationBuilder.CreateIndex(
                name: "IX_Socios_IdCiudad",
                table: "Socios",
                column: "IdCiudad");

            migrationBuilder.CreateIndex(
                name: "IX_Socios_IdEstadoSocio",
                table: "Socios",
                column: "IdEstadoSocio");

            migrationBuilder.CreateIndex(
                name: "IX_Socios_IdUsuario",
                table: "Socios",
                column: "IdUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tiendas_IdCategoria",
                table: "Tiendas",
                column: "IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_Tiendas_IdCiudad",
                table: "Tiendas",
                column: "IdCiudad");

            migrationBuilder.CreateIndex(
                name: "IX_Tiendas_IdEstadoTienda",
                table: "Tiendas",
                column: "IdEstadoTienda");

            migrationBuilder.CreateIndex(
                name: "IX_TokensEmail_IdTipoTokenEmail",
                table: "TokensEmail",
                column: "IdTipoTokenEmail");

            migrationBuilder.CreateIndex(
                name: "IX_TokensEmail_IdUsuario",
                table: "TokensEmail",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_TokensQR_IdMembresia",
                table: "TokensQR",
                column: "IdMembresia");

            migrationBuilder.CreateIndex(
                name: "IX_UsosPromocion_IdMembresia",
                table: "UsosPromocion",
                column: "IdMembresia");

            migrationBuilder.CreateIndex(
                name: "IX_UsosPromocion_IdPromocion",
                table: "UsosPromocion",
                column: "IdPromocion");

            migrationBuilder.CreateIndex(
                name: "IX_UsosPromocion_IdUsuarioTienda",
                table: "UsosPromocion",
                column: "IdUsuarioTienda");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdEstadoUsuario",
                table: "Usuarios",
                column: "IdEstadoUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_NomUsuario",
                table: "Usuarios",
                column: "NomUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosTienda_IdEstadoUsuarioTienda",
                table: "UsuariosTienda",
                column: "IdEstadoUsuarioTienda");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosTienda_IdRolTienda",
                table: "UsuariosTienda",
                column: "IdRolTienda");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosTienda_IdTienda",
                table: "UsuariosTienda",
                column: "IdTienda");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosTienda_IdUsuario",
                table: "UsuariosTienda",
                column: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cupones");

            migrationBuilder.DropTable(
                name: "PagosMembresiaMercadoPago");

            migrationBuilder.DropTable(
                name: "TokensEmail");

            migrationBuilder.DropTable(
                name: "TokensQR");

            migrationBuilder.DropTable(
                name: "UsosPromocion");

            migrationBuilder.DropTable(
                name: "EstadosCupon");

            migrationBuilder.DropTable(
                name: "EstadosPagoMembresia");

            migrationBuilder.DropTable(
                name: "TiposTokenEmail");

            migrationBuilder.DropTable(
                name: "Membresias");

            migrationBuilder.DropTable(
                name: "Promociones");

            migrationBuilder.DropTable(
                name: "UsuariosTienda");

            migrationBuilder.DropTable(
                name: "EstadosMembresia");

            migrationBuilder.DropTable(
                name: "Socios");

            migrationBuilder.DropTable(
                name: "EstadosPromocion");

            migrationBuilder.DropTable(
                name: "EstadosUsuarioTienda");

            migrationBuilder.DropTable(
                name: "RolesTienda");

            migrationBuilder.DropTable(
                name: "Tiendas");

            migrationBuilder.DropTable(
                name: "EstadosSocio");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Ciudades");

            migrationBuilder.DropTable(
                name: "EstadosTienda");

            migrationBuilder.DropTable(
                name: "EstadosUsuario");

            migrationBuilder.DropTable(
                name: "Provincias");
        }
    }
}
