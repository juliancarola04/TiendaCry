using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Data.Configuraciones
{
    public class EstadoMembresiaConfiguracion : IEntityTypeConfiguration<EstadoMembresia>
    {
        public void Configure(EntityTypeBuilder<EstadoMembresia> builder)
        {
            builder.ToTable("EstadosMembresia");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nombre)
                .IsRequired();
        }
    }
}
