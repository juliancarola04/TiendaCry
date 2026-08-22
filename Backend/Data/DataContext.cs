using Backend.Data.Configuraciones;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UsuarioConfiguracion());
            modelBuilder.ApplyConfiguration(new EstadoUsuarioConfiguracion());
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<EstadoUsuario> EstadosUsuario{ get; set; }

    }
}
