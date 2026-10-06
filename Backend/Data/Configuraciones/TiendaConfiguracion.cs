using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configuraciones
{
    public class TiendaConfiguracion : IEntityTypeConfiguration<Tienda>
    {
        public void Configure(EntityTypeBuilder<Tienda> builder)
        {
            builder.ToTable("Tiendas");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.FechaRevision)
                .IsRequired(false)
                .HasColumnType("timestamp with time zone");

            builder.Property(t => t.MotivoRechazo)
                .IsRequired(false);

            builder.Property(t => t.Nombre)
                .IsRequired();

            builder.Property(t => t.Calle)
                .IsRequired();

            builder.Property(t => t.NumCalle)
                .IsRequired();

            builder.Property(t => t.FechaSolicitud)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.HasOne(t => t.Ciudad)
                .WithMany(c => c.Tiendas)
                .HasForeignKey(t => t.IdCiudad)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.EstadoTienda)
                .WithMany(e => e.Tiendas)
                .HasForeignKey(t => t.IdEstadoTienda)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Categoria)
                .WithMany(c => c.Tiendas)
                .HasForeignKey(t => t.IdCategoria)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
