using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Data.Configuraciones
{
    public class EstadoUsuarioTiendaConfiguracion : IEntityTypeConfiguration<EstadoUsuarioTienda>
    {
        public void Configure(EntityTypeBuilder<EstadoUsuarioTienda> builder)
        {
            builder.ToTable("EstadosUsuarioTienda");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nombre)
                .IsRequired();
        }
    }
}
