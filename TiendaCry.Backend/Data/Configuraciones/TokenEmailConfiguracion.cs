using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Data.Configuraciones
{
    public class TokenEmailConfiguracion : IEntityTypeConfiguration<TokenEmail>
    {
        public void Configure(EntityTypeBuilder<TokenEmail> builder)
        {
            builder.ToTable("TokensEmail");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.EmailNuevo)
                .IsRequired(false);

            builder.Property(t => t.Codigo)
                .IsRequired();

            builder.Property(t => t.FechaCreacion)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(t => t.FechaVencimiento)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(t => t.Usado)
                .IsRequired()
                .IsConcurrencyToken();

            builder.HasOne(t => t.TipoTokenEmail)
                .WithMany(t => t.TokensEmail)
                .HasForeignKey(t => t.IdTipoTokenEmail)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Usuario)
                .WithMany(u => u.TokensEmail)
                .HasForeignKey(t => t.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
