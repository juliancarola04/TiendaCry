using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Backend.Data.Configuraciones
{
    public class EstadoUsuarioConfiguracion : IEntityTypeConfiguration<EstadoUsuario>
    {
        public void Configure(EntityTypeBuilder<EstadoUsuario> builder)
        {
            builder.ToTable("EstadosUsuario");

            builder.HasKey(eu => eu.Id);

            builder.Property(eu => eu.Nombre)
                .IsRequired()
                .HasMaxLength(30);

            builder.HasIndex(eu => eu.Nombre)
                .IsUnique();
        }
    }
}
