using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Data.Configuraciones
{
    public class EstadoTiendaConfiguracion : IEntityTypeConfiguration<EstadoTienda>
    {
        public void Configure(EntityTypeBuilder<EstadoTienda> builder)
        {
            builder.ToTable("EstadosTienda");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nombre)
                .IsRequired();
        }
    }
}
