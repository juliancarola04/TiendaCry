using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Data.Configuraciones
{
    public class EstadoSocioConfiguracion : IEntityTypeConfiguration<EstadoSocio>
    {
        public void Configure(EntityTypeBuilder<EstadoSocio> builder)
        {
            builder.ToTable("EstadosSocio");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nombre)
                .IsRequired();
        }
    }
}
