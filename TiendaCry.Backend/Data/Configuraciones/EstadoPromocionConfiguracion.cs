using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Data.Configuraciones
{
    public class EstadoPromocionConfiguracion : IEntityTypeConfiguration<EstadoPromocion>
    {
        public void Configure(EntityTypeBuilder<EstadoPromocion> builder)
        {
            builder.ToTable("EstadosPromocion");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nombre)
                .IsRequired();
        }
    }
}
