using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Data.Configuraciones
{
    public class EstadoCuponConfiguracion : IEntityTypeConfiguration<EstadoCupon>
    {
        public void Configure(EntityTypeBuilder<EstadoCupon> builder)
        {
            builder.ToTable("EstadosCupon");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nombre)
                .IsRequired();
        }
    }
}
