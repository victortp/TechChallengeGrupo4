using ContatosGrupo4.Infrastructure.Data.Contexts;
using ContatosGrupo4.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace ContatosGrupo4.Tests.Integration
{
    public class SqlServerTests : IAsyncLifetime
    {
        private readonly MsSqlContainer _dbContainer = new MsSqlBuilder().Build();
        private AppDbContext _dbContext = null!;
        public ContatoRepository contatoRepository = null!;

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(_dbContainer.GetConnectionString())
                .Options;

            _dbContext = new AppDbContext(options);

            if (_dbContext.Database.GetPendingMigrations().Any())
            {
                _dbContext.Database.Migrate();
            };

            contatoRepository = new ContatoRepository(_dbContext);
        }

        public Task DisposeAsync()
        {
            return _dbContainer.DisposeAsync().AsTask();
        }
    }
}
