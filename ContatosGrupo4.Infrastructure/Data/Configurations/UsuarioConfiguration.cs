using System.Diagnostics.CodeAnalysis;
using ContatosGrupo4.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContatosGrupo4.Infrastructure.Data.Configurations
{
    [ExcludeFromCodeCoverage]
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuario");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasColumnType("int")
                .UseIdentityColumn();
            builder.Property(x => x.DataCriacao)
                .HasColumnType("datetime")
                .IsRequired();
            builder.Property(x => x.DataAtualizacao)
                .HasColumnType("datetime")
                .IsRequired();
            builder.Property(x => x.Login)
                .HasColumnType("varchar(100)")
                .IsRequired();
            builder.HasIndex(x => x.Login)
                .IsUnique();
            builder.Property(x => x.Senha)
                .HasColumnType("varchar(100)")
                .IsRequired();
            builder.HasMany(u => u.Contato)
                .WithOne(c => c.Usuario)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.ClientNoAction);
        }
    }
}
