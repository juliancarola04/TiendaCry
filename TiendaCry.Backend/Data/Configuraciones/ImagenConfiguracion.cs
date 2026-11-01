using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Data.Configuraciones
{
    public class ImagenConfiguracion : IEntityTypeConfiguration<Imagen>
    {
        public void Configure(EntityTypeBuilder<Imagen> builder)
        {
            builder.ToTable("Imagenes");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.NombreOriginal).IsRequired();
            builder.Property(i => i.NombreArchivo).IsRequired();
            builder.Property(i => i.RutaRelativa).IsRequired();
            builder.Property(i => i.TipoContenido).IsRequired();
            builder.Property(i => i.TamanoBytes).IsRequired();

            builder.Property(i => i.FechaCreacion)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.HasOne(i => i.Promocion)
                .WithOne(p => p.Imagen)
                .HasForeignKey<Imagen>(i => i.IdPromocion)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(i => i.IdPromocion).IsUnique();
        }
    }
}
