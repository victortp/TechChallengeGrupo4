using System.Diagnostics.CodeAnalysis;
using ContatosGrupo4.Application.Configurations;
using ContatosGrupo4.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ContatosGrupo4.Infrastructure.Data.Contexts
{
    public class AppDbContext(IOptions<DatabaseSettings> databaseSettings) : DbContext
    {
        private readonly IOptions<DatabaseSettings> _databaseSettings = databaseSettings;

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

    [ExcludeFromCodeCoverage]
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.FullName, "ContatosGrupo4.Api");

            if (!Directory.Exists(basePath))
            {
                throw new DirectoryNotFoundException($"O diretório base '{basePath}' não foi encontrado.");
            }

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var databaseSettings = new DatabaseSettings
            {
                ConnectionString = configuration.GetValue<string>("SqlServer:ConnectionString")
            };

            var options = Options.Create(databaseSettings);

            return new AppDbContext(options);
        }
    }
}
