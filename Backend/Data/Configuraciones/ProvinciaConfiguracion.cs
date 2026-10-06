using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Backend.Data.Configuraciones
{
    public class ProvinciaConfiguracion : IEntityTypeConfiguration<Provincia>
    {
        public void Configure(EntityTypeBuilder<Provincia> builder)
        {
            builder.ToTable("Provincias");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nombre)
                .IsRequired();
        }


    }
}
