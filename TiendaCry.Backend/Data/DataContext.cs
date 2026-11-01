using Microsoft.EntityFrameworkCore;
using TiendaCry.Backend.Data.Configuraciones;
using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UsuarioConfiguracion());
            modelBuilder.ApplyConfiguration(new SesionUsuarioConfiguracion());
            modelBuilder.ApplyConfiguration(new EstadoUsuarioConfiguracion());
            modelBuilder.ApplyConfiguration(new ProvinciaConfiguracion());
            modelBuilder.ApplyConfiguration(new CiudadConfiguracion());
            modelBuilder.ApplyConfiguration(new EstadoMembresiaConfiguracion());
            modelBuilder.ApplyConfiguration(new EstadoCuponConfiguracion());
            modelBuilder.ApplyConfiguration(new EstadoPagoMembresiaConfiguracion());
            modelBuilder.ApplyConfiguration(new EstadoUsuarioTiendaConfiguracion());
            modelBuilder.ApplyConfiguration(new RolTiendaConfiguracion());
            modelBuilder.ApplyConfiguration(new EstadoTiendaConfiguracion());
            modelBuilder.ApplyConfiguration(new EstadoPromocionConfiguracion());
            modelBuilder.ApplyConfiguration(new CategoriaConfiguracion());
            modelBuilder.ApplyConfiguration(new TipoTokenEmailConfiguracion());
            modelBuilder.ApplyConfiguration(new SocioConfiguracion());
            modelBuilder.ApplyConfiguration(new MembresiaConfiguracion());
            modelBuilder.ApplyConfiguration(new CuponConfiguracion());
            modelBuilder.ApplyConfiguration(new TokenQRConfiguracion());
            modelBuilder.ApplyConfiguration(new TokenEmailConfiguracion());
            modelBuilder.ApplyConfiguration(new PagoMembresiaMercadoPagoConfiguracion());
            modelBuilder.ApplyConfiguration(new TiendaConfiguracion());
            modelBuilder.ApplyConfiguration(new PromocionConfiguracion());
            modelBuilder.ApplyConfiguration(new UsoPromocionConfiguracion());
            modelBuilder.ApplyConfiguration(new UsuarioTiendaConfiguracion());
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<SesionUsuario> SesionesUsuario { get; set; }
        public DbSet<EstadoUsuario> EstadosUsuario{ get; set; }
        public DbSet<Provincia> Provincias { get; set; }
        public DbSet<Ciudad> Ciudades { get; set; }
        public DbSet<EstadoMembresia> EstadosMembresia { get; set; }
        public DbSet<EstadoCupon> EstadosCupon { get; set; }
        public DbSet<EstadoPagoMembresia> EstadosPagoMembresia { get; set; }
        public DbSet<EstadoUsuarioTienda> EstadosUsuarioTienda { get; set; }
        public DbSet<RolTienda> RolesTienda { get; set; }
        public DbSet<EstadoTienda> EstadosTienda { get; set; }
        public DbSet<EstadoPromocion> EstadosPromocion { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<TipoTokenEmail> TiposTokenEmail { get; set; }
        public DbSet<Socio> Socios { get; set; }
        public DbSet<Membresia> Membresias { get; set; }
        public DbSet<Cupon> Cupones { get; set; }
        public DbSet<TokenQR> TokensQR { get; set; }
        public DbSet<TokenEmail> TokensEmail { get; set; }
        public DbSet<PagoMembresiaMercadoPago> PagosMembresiaMercadoPago { get; set; }
        public DbSet<Tienda> Tiendas { get; set; }
        public DbSet<Promocion> Promociones { get; set; }
        public DbSet<UsoPromocion> UsosPromocion { get; set; }
        public DbSet<UsuarioTienda> UsuariosTienda { get; set; }

    }
}
