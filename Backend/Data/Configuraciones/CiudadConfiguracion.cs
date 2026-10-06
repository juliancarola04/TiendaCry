using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configuraciones
{
    public class CiudadConfiguracion : IEntityTypeConfiguration<Ciudad>
    {
        public void Configure(EntityTypeBuilder<Ciudad> builder)
        {
            builder.ToTable("Ciudades");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nombre)
                .IsRequired();

            builder.HasOne(c => c.Provincia)
                .WithMany(p => p.Ciudades)
                .HasForeignKey(c => c.ProvinciaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
