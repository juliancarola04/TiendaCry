using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Data.Configuraciones
{
    public class UsoPromocionConfiguracion : IEntityTypeConfiguration<UsoPromocion>
    {
        public void Configure(EntityTypeBuilder<UsoPromocion> builder)
        {
            builder.ToTable("UsosPromocion");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.FechaHoraUso)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.HasOne(u => u.Membresia)
                .WithMany(m => m.UsosPromocion)
                .HasForeignKey(u => u.IdMembresia)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.Promocion)
                .WithMany(p => p.UsosPromocion)
                .HasForeignKey(u => u.IdPromocion)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.UsuarioTienda)
                .WithMany(u => u.UsosPromocion)
                .HasForeignKey(u => u.IdUsuarioTienda)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
