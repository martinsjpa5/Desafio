using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infraestrutura.EF.Configurations
{
    public class CompraConfiguration : IEntityTypeConfiguration<Compra>
    {
        public void Configure(EntityTypeBuilder<Compra> builder)
        {
            builder.HasKey(v => v.Id);

            builder.Property(v => v.ValorTotal)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(v => v.DataCriacao).IsRequired();
            builder.Property(v => v.Cancelada).IsRequired();

            builder.HasMany(v => v.Itens)
                   .WithOne(pv => pv.Compra)
                   .HasForeignKey("CompraId");

            builder.HasOne<IdentityUser>()
                   .WithMany()
                   .HasForeignKey(v => v.UsuarioId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
