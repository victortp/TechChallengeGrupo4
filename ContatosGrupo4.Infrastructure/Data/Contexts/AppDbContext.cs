using ContatosGrupo4.Application.Configurations;
using ContatosGrupo4.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ContatosGrupo4.Infrastructure.Data.Contexts
{
    public class AppDbContext : DbContext
    {
        private readonly IOptions<DatabaseSettings> _databaseSettings;

        public AppDbContext(IOptions<DatabaseSettings> databaseSettings)
        {
            _databaseSettings = databaseSettings;
        }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Contato> Contato { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseSqlServer(_databaseSettings.Value.ConnectionString);
            } 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
