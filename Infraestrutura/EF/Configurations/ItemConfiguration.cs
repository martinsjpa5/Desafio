using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infraestrutura.EF.Configurations
{
    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.HasKey(pv => pv.Id);

            builder.Property(pv => pv.Quantidade)
                   .IsRequired();

            builder.Property(pv => pv.Valor)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.HasOne(pv => pv.Produto)
                   .WithMany()
                   .HasForeignKey("ProdutoId")
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pv => pv.Compra)
                   .WithMany(v => v.Itens)
                   .HasForeignKey("CompraId")
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(pv => pv.DataCriacao).IsRequired();
        }
    }
}
