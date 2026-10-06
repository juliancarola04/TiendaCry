using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configuraciones
{
    public class MembresiaConfiguracion : IEntityTypeConfiguration<Membresia>
    {
        public void Configure(EntityTypeBuilder<Membresia> builder)
        {
            builder.ToTable("Membresias");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.FechaFin)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(m => m.MontoMensualCupones)
                .IsRequired();

            builder.Property(m => m.FechaInicio)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.HasOne(m => m.Socio)
                .WithMany(s => s.Membresias)
                .HasForeignKey(m => m.IdSocio)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.EstadoMembresia)
                .WithMany(e => e.Membresias)
                .HasForeignKey(m => m.IdEstadoMembresia)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
