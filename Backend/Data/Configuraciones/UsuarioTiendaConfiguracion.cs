using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configuraciones
{
    public class UsuarioTiendaConfiguracion : IEntityTypeConfiguration<UsuarioTienda>
    {
        public void Configure(EntityTypeBuilder<UsuarioTienda> builder)
        {
            builder.ToTable("UsuariosTienda");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.FechaBaja)
                .IsRequired(false)
                .HasColumnType("timestamp with time zone");

            builder.Property(u => u.FechaAlta)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.HasOne(u => u.Usuario)
                .WithMany(u => u.UsuariosTienda)
                .HasForeignKey(u => u.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.Tienda)
                .WithMany(t => t.UsuariosTienda)
                .HasForeignKey(u => u.IdTienda)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.RolTienda)
                .WithMany(r => r.UsuariosTienda)
                .HasForeignKey(u => u.IdRolTienda)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.EstadoUsuarioTienda)
                .WithMany(e => e.UsuariosTienda)
                .HasForeignKey(u => u.IdEstadoUsuarioTienda)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
