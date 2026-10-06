using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configuraciones
{
    public class SocioConfiguracion : IEntityTypeConfiguration<Socio>
    {
        public void Configure(EntityTypeBuilder<Socio> builder)
        {
            builder.ToTable("Socios");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.NumSocio)
                .IsRequired();

            builder.HasOne(s => s.Usuario)
                .WithOne(u => u.Socio)
                .HasForeignKey<Socio>(s => s.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.EstadoSocio)
                .WithMany(e => e.Socios)
                .HasForeignKey(s => s.IdEstadoSocio)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Ciudad)
                .WithMany(c => c.Socios)
                .HasForeignKey(s => s.IdCiudad)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
