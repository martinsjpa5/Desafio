using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infraestrutura.EF.Configurations
{
    public class AvaliacaoConfiguration : IEntityTypeConfiguration<Avaliacao>
    {
        public void Configure(EntityTypeBuilder<Avaliacao> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Nota)
                   .IsRequired();

            builder.Property(a => a.Comentario)
                   .HasMaxLength(500);

            builder.Property(a => a.DataCriacao).IsRequired();

            builder.HasOne(a => a.Produto)
                   .WithMany(p => p.Avaliacoes)
                   .HasForeignKey("ProdutoId")
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<IdentityUser>()
                   .WithMany()
                   .HasForeignKey(v => v.UsuarioId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
