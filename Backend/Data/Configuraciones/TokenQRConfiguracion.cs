using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configuraciones
{
    public class TokenQRConfiguracion : IEntityTypeConfiguration<TokenQR>
    {
        public void Configure(EntityTypeBuilder<TokenQR> builder)
        {
            builder.ToTable("TokensQR");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Token)
                .IsRequired();

            builder.Property(t => t.FechaGeneracion)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(t => t.FechaExpiracion)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.HasOne(t => t.Membresia)
                .WithMany(m => m.TokensQR)
                .HasForeignKey(t => t.IdMembresia)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
