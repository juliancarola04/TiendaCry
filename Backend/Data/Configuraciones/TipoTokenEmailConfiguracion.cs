using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configuraciones
{
    public class TipoTokenEmailConfiguracion : IEntityTypeConfiguration<TipoTokenEmail>
    {
        public void Configure(EntityTypeBuilder<TipoTokenEmail> builder)
        {
            builder.ToTable("TiposTokenEmail");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nombre)
                .IsRequired();
        }
    }
}
