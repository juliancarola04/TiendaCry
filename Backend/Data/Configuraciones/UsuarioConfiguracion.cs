using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configuraciones
{
    public class UsuarioConfiguracion : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.NomUsuario)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(u => u.Contrasena)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(254);

            builder.Property(u => u.EmailVerificado)
                .IsRequired();

            builder.Property(u => u.Nombre)
                .IsRequired()
                .HasMaxLength(35);

            builder.Property(u => u.Apellido)
                .IsRequired()
                .HasMaxLength(35);

            builder.Property(u => u.EsAdministradorTiendaCry)
                .IsRequired();

            builder.Property(u => u.DebeCambiarContrasena)
                .IsRequired();

            builder.HasOne(u => u.EstadoUsuario)
                .WithMany(eu => eu.Usuarios)
                .HasForeignKey(u => u.IdEstadoUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(u => u.NomUsuario)
                .IsUnique();

            builder.HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
