using ContatosGrupo4.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContatosGrupo4.Infrastructure.Data.Configurations
{
    public class ContatoConfiguration : IEntityTypeConfiguration<Contato>
    {
        public void Configure(EntityTypeBuilder<Contato> builder)
        {
            builder.ToTable("Contato");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasColumnType("int")
                .ValueGeneratedNever()
                .UseIdentityColumn();
            builder.Property(x => x.DataCriacao)
                .HasColumnType("datetime")
                .IsRequired();
            builder.Property(x => x.DataAtualizacao)
                .HasColumnType("datetime")
                .IsRequired();
            builder.Property(c => c.Nome)
              .HasColumnType("varchar(100)")
              .IsRequired();
            builder.Property(c => c.Telefone)
                   .HasColumnType("varchar(15)")
                   .IsRequired();
            builder.Property(c => c.Email)
                   .HasColumnType("varchar(200)")
                   .IsRequired();
        }
    }
}
