using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configuraciones
{
    public class EstadoPagoMembresiaConfiguracion : IEntityTypeConfiguration<EstadoPagoMembresia>
    {
        public void Configure(EntityTypeBuilder<EstadoPagoMembresia> builder)
        {
            builder.ToTable("EstadosPagoMembresia");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nombre)
                .IsRequired();
        }
    }
}
