using ContatosGrupo4.Application.Configurations;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ContatosGrupo4.Infrastructure.Data.Contexts
{
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
