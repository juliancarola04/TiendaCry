using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Data.Configuraciones
{
    public class RolTiendaConfiguracion : IEntityTypeConfiguration<RolTienda>
    {
        public void Configure(EntityTypeBuilder<RolTienda> builder)
        {
            builder.ToTable("RolesTienda");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nombre)
                .IsRequired();
        }
    }
}
