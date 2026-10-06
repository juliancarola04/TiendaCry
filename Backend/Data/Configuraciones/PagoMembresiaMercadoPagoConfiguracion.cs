using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configuraciones
{
    public class PagoMembresiaMercadoPagoConfiguracion : IEntityTypeConfiguration<PagoMembresiaMercadoPago>
    {
        public void Configure(EntityTypeBuilder<PagoMembresiaMercadoPago> builder)
        {
            builder.ToTable("PagosMembresiaMercadoPago");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.FechaPago)
                .IsRequired(false)
                .HasColumnType("timestamp with time zone");

            builder.Property(p => p.IdPagoMP)
                .IsRequired(false);

            builder.Property(p => p.Monto)
                .IsRequired();

            builder.Property(p => p.Moneda)
                .IsRequired();

            builder.Property(p => p.FechaCreacion)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(p => p.IdPreferenciaMP)
                .IsRequired();

            builder.Property(p => p.ExternalReferenceMP)
                .IsRequired();

            builder.HasOne(p => p.EstadoPagoMembresia)
                .WithMany(e => e.PagosMembresiaMercadoPago)
                .HasForeignKey(p => p.IdEstadoPagoMembresia)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Membresia)
                .WithMany(m => m.PagosMembresiaMercadoPago)
                .HasForeignKey(p => p.IdMembresia)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.IdPagoMP)
                .IsUnique()
                .HasFilter("\"IdPagoMP\" IS NOT NULL");
        }
    }
}
