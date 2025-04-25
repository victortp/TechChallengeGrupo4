using System.Text;
using System.Text.Json;
using ContatosGrupo4.Application.Configurations;
using ContatosGrupo4.Infrastructure.Data.Contexts;
using ContatosGrupo4.Infrastructure.Data.Repositories;
using ContatosGrupo4.Infrastructure.Messaging.Producers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;

namespace ContatosGrupo4.Tests.Integration
{
    public class IntegrationTestsFixture : IAsyncLifetime
    {
        private readonly MsSqlContainer _dbContainer = new MsSqlBuilder().Build();
        private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder().Build();
        private AppDbContext _dbContext = null!;
        public ContatoRepository ContatoRepository = null!;
        public IMemoryCache MemoryCache = null!;
        private ConnectionFactory _connectionFactory = null!;
        public RabbitMQPublisher RabbitMQPublisher = null!;
        public IOptions<RabbitMQOptions> RabbitMqOptions = null!;

        public async Task InitializeAsync()
        {
            // banco de dados
            await _dbContainer.StartAsync();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(_dbContainer.GetConnectionString())
                .Options;

            _dbContext = new AppDbContext(options);

            if (_dbContext.Database.GetPendingMigrations().Any())
            {
                _dbContext.Database.Migrate();
            };

            ContatoRepository = new ContatoRepository(_dbContext);

            // cache
            MemoryCache = new MemoryCache(new MemoryCacheOptions());

            // rabbit
            await _rabbitMqContainer.StartAsync();

            _connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri(_rabbitMqContainer.GetConnectionString())
            };

            RabbitMQPublisher = new RabbitMQPublisher(_connectionFactory);

            var rabbitMqOptions = new RabbitMQOptions
            {
                HostName = string.Empty,
                UserName = string.Empty,
                Password = string.Empty,
                Queues = new RabbitMQQueues
                {
                    CriarContato = "contato-criar-queue",
                    AtualizarContato = "contato-atualizar-queue",
                    ExcluirContato = "contato-excluir-queue"
                }
            };

            RabbitMqOptions = Options.Create(rabbitMqOptions);
        }

        public async Task DisposeAsync()
        {
            await _dbContainer.DisposeAsync();
            await _rabbitMqContainer.DisposeAsync();
        }

        public async Task<T?> ConsumeMessageAsync<T>(string queueName, TimeSpan timeout) where T : class
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            string? mensagem = null;
            var waitingHandle = new ManualResetEventSlim(false);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += (sender, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                mensagem = Encoding.UTF8.GetString(body);
                waitingHandle.Set();

                return Task.CompletedTask;
            };

            var consumerTag = await channel.BasicConsumeAsync(queueName, true, consumer: consumer);

            if (waitingHandle.Wait(timeout))
            {
                await channel.BasicCancelAsync(consumerTag);
                return JsonSerializer.Deserialize<T>(mensagem!);
            }

            await channel.BasicCancelAsync(consumerTag);

            return null;
        }
    }
}
