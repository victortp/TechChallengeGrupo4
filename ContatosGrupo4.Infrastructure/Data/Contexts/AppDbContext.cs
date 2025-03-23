using System.Diagnostics.CodeAnalysis;
using ContatosGrupo4.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ContatosGrupo4.Infrastructure.Data.Contexts
{
    [ExcludeFromCodeCoverage]
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)

    {
        public DbSet<Contato> Contato { get; set; }

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

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetValue<string>("SqlServer:ConnectionString"));

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
