using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Data.Configuraciones
{
    public class SesionUsuarioConfiguracion : IEntityTypeConfiguration<SesionUsuario>
    {
        public void Configure(EntityTypeBuilder<SesionUsuario> builder)
        {
            builder.ToTable("SesionesUsuario");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.RefreshToken)
                .IsRequired()
                .HasMaxLength(64)
                .IsConcurrencyToken();

            builder.HasIndex(s => s.RefreshToken)
                .IsUnique();

            builder.Property(s => s.FechaCreacion)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(s => s.FechaVencimiento)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(s => s.FechaRevocacion)
                .HasColumnType("timestamp with time zone")
                .IsRequired(false);

            builder.HasOne(s => s.Usuario)
                .WithMany()
                .HasForeignKey(s => s.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
