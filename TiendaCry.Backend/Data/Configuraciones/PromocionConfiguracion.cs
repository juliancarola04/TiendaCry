using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Data.Configuraciones
{
    public class PromocionConfiguracion : IEntityTypeConfiguration<Promocion>
    {
        public void Configure(EntityTypeBuilder<Promocion> builder)
        {
            builder.ToTable("Promociones");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.FechaRevision)
                .IsRequired(false)
                .HasColumnType("timestamp with time zone");

            builder.Property(p => p.MotivoRechazo)
                .IsRequired(false);

            builder.Property(p => p.Titulo)
                .IsRequired();

            builder.Property(p => p.Descripcion)
                .IsRequired();

            builder.Property(p => p.FechaInicio)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(p => p.FechaFin)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(p => p.FechaSolicitud)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.HasOne(p => p.Tienda)
                .WithMany(t => t.Promociones)
                .HasForeignKey(p => p.IdTienda)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.EstadoPromocion)
                .WithMany(e => e.Promociones)
                .HasForeignKey(p => p.IdEstadoPromocion)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
