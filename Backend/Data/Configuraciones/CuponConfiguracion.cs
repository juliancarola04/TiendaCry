using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configuraciones
{
    public class CuponConfiguracion : IEntityTypeConfiguration<Cupon>
    {
        public void Configure(EntityTypeBuilder<Cupon> builder)
        {
            builder.ToTable("Cupones");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.FechaUso)
                .IsRequired(false)
                .HasColumnType("timestamp with time zone");

            builder.Property(c => c.IdTiendaNube)
                .IsRequired();

            builder.Property(c => c.Monto)
                .IsRequired();

            builder.Property(c => c.FechaInicio)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(c => c.FechaFin)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.HasOne(c => c.EstadoCupon)
                .WithMany(e => e.Cupones)
                .HasForeignKey(c => c.IdEstadoCupon)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Membresia)
                .WithMany(m => m.Cupones)
                .HasForeignKey(c => c.IdMembresia)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
