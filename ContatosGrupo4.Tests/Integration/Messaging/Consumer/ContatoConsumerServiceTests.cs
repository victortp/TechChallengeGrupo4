using System.Text;
using System.Text.Json;
using ContatosGrupo4.Application.Configurations;
using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using ContatosGrupo4.Infrastructure.Data.Contexts;
using ContatosGrupo4.Infrastructure.Data.Repositories;
using ContatosGrupo4.Infrastructure.Messaging.Consumers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace ContatosGrupo4.Tests.Integration.Messaging.Consumer
{
    public class ContatoConsumerServiceTests : IClassFixture<IntegrationTestsFixture>
    {
        private readonly ContatoRepository _repository;
        private readonly IntegrationTestsFixture _fixture;
        private readonly IServiceScopeFactory _scopeFactory;

        public ContatoConsumerServiceTests(IntegrationTestsFixture fixture)
        {
            _fixture = fixture;
            _repository = fixture.ContatoRepository;
            var services = new ServiceCollection();
            services.AddSingleton(_fixture.ConnectionFactory);
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(_fixture.DbContainer.GetConnectionString()));
            services.AddScoped<IContatoRepository, ContatoRepository>();

            services.AddOptions<RabbitMQOptions>().Configure(o =>
            {
                o.Queues = new RabbitMQQueues
                {
                    CriarContato = $"integ-test-create-{Guid.NewGuid()}",
                    AtualizarContato = $"integ-test-update-{Guid.NewGuid()}",
                    ExcluirContato = $"integ-test-delete-{Guid.NewGuid()}"
                };
            });

            services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
            services.AddMemoryCache();

            var serviceProvider = services.BuildServiceProvider();
            _scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        }

        private async Task PublicarMensagemAsync(string queueName, object message)
        {
            using var connection = await _fixture.ConnectionFactory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);
            var properties = new BasicProperties { Persistent = true };
            properties.Persistent = true;
            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, mandatory: false, basicProperties: properties, body: body);
        }

        private async Task<bool> FoiProcessada(string queueName, CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);
            try
            {
                using var checkConnection = await _fixture.ConnectionFactory.CreateConnectionAsync(cancellationToken);
                using var checkChannel = await checkConnection.CreateChannelAsync(cancellationToken: cancellationToken);
                await checkChannel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);
                var getResult = await checkChannel.BasicGetAsync(queueName, false);
                return getResult == null;
            }
            catch (Exception)
            {
                return false;
            }
        }


        [Fact]
        public async Task Consumer_DeveProcessarMensagemDeCriacaoValida_EAdicionarNoBanco()
        {
            using var scope = _scopeFactory.CreateScope();
            var options = scope.ServiceProvider.GetRequiredService<IOptions<RabbitMQOptions>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ContatoConsumerService>>();
            var queueName = options.Value.Queues.CriarContato;

            var consumerService = new ContatoConsumerService(logger, _scopeFactory, _fixture.ConnectionFactory, options); 
            var dto = new CriarContatoDto { Nome = $"Teste {Guid.NewGuid()}", Email = $"teste-{Guid.NewGuid()}@test.com", Telefone = "9876543210" };
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

            await consumerService.StartAsync(cts.Token);
            await PublicarMensagemAsync(queueName, dto);

            Contato? contatoCriado = null;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            while (contatoCriado == null && stopwatch.Elapsed < TimeSpan.FromSeconds(15) && !cts.IsCancellationRequested)
            {
                await Task.Delay(250, cts.Token);
                using var checkScope = _scopeFactory.CreateScope();
                var checkRepo = checkScope.ServiceProvider.GetRequiredService<IContatoRepository>();
                contatoCriado = await checkRepo.ObterPorNomeEmailAsync(dto.Nome, dto.Email);
            }
            stopwatch.Stop();

            contatoCriado.Should().NotBeNull();
            contatoCriado!.Nome.Should().Be(dto.Nome);

            var processou = await FoiProcessada(queueName, cts.Token);
            processou.Should().BeTrue();

            await consumerService.StopAsync(CancellationToken.None);
            consumerService.Dispose();
        }

        [Fact]
        public async Task Consumer_DeveProcessarMensagemDeAtualizacaoValida_EAtualizarOBanco()
        {
            using var scope = _scopeFactory.CreateScope();
            var repoInicial = scope.ServiceProvider.GetRequiredService<IContatoRepository>();
            var options = scope.ServiceProvider.GetRequiredService<IOptions<RabbitMQOptions>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ContatoConsumerService>>();
            var queueName = options.Value.Queues.AtualizarContato;

            var contatoInicial = new Contato { Nome = $"Teste", Email = $"teste-{Guid.NewGuid()}@test.com", Telefone = "9876543210" };
            contatoInicial.SetDataCriacao();
            await repoInicial.AdicionarAsync(contatoInicial);
            contatoInicial.Id.Should().BeGreaterThan(0);

            var consumerService = new ContatoConsumerService(logger, _scopeFactory, _fixture.ConnectionFactory, options);
            var dto = new AtualizarContatoDto { Id = contatoInicial.Id, Nome = $"Teste Atualizado", Email = $"teste-{Guid.NewGuid()}@test.com", Telefone = "9876543210" };
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

            await consumerService.StartAsync(cts.Token);
            await PublicarMensagemAsync(queueName, dto);

            Contato? contatoAtualizado = null;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            while (contatoAtualizado?.Nome != dto.Nome && stopwatch.Elapsed < TimeSpan.FromSeconds(15) && !cts.IsCancellationRequested)
            {
                await Task.Delay(250, cts.Token);
                using var checkScope = _scopeFactory.CreateScope();
                var checkRepo = checkScope.ServiceProvider.GetRequiredService<IContatoRepository>();
                contatoAtualizado = await checkRepo.ObterPorIdAsync(dto.Id);
            }
            stopwatch.Stop();

            contatoAtualizado.Should().NotBeNull();
            contatoAtualizado!.Nome.Should().Be(dto.Nome);
            contatoAtualizado.Email.Should().Be(dto.Email);
            contatoAtualizado.Telefone.Should().Be(dto.Telefone);
            contatoAtualizado.DataAtualizacao.Should().BeAfter(contatoInicial.DataCriacao);

            var processou = await FoiProcessada(queueName, cts.Token);
            processou.Should().BeTrue();

            await consumerService.StopAsync(CancellationToken.None);
            consumerService.Dispose();
        }

        [Fact]
        public async Task Consumer_DeveProcessarMensagemDeExclusaoValida_ERemoverDoBanco()
        {
            using var scope = _scopeFactory.CreateScope();
            var repoInicial = scope.ServiceProvider.GetRequiredService<IContatoRepository>();
            var options = scope.ServiceProvider.GetRequiredService<IOptions<RabbitMQOptions>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ContatoConsumerService>>();
            var queueName = options.Value.Queues.ExcluirContato;

            var contatoInicial = new Contato { Nome = $"Teste {Guid.NewGuid()}", Email = $"teste-{Guid.NewGuid()}@test.com", Telefone = "9876543210" };
            contatoInicial.SetDataCriacao();
            await repoInicial.AdicionarAsync(contatoInicial);
            contatoInicial.Id.Should().BeGreaterThan(0);

            var consumerService = new ContatoConsumerService(logger, _scopeFactory, _fixture.ConnectionFactory, options);
            var dto = new ExcluirContatoDto { Id = contatoInicial.Id };
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

            await consumerService.StartAsync(cts.Token);
            await PublicarMensagemAsync(queueName, dto);

            Contato? contatoExcluido = contatoInicial;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            while (contatoExcluido != null && stopwatch.Elapsed < TimeSpan.FromSeconds(15) && !cts.IsCancellationRequested)
            {
                await Task.Delay(250, cts.Token);
                using var checkScope = _scopeFactory.CreateScope();
                var checkRepo = checkScope.ServiceProvider.GetRequiredService<IContatoRepository>();
                contatoExcluido = await checkRepo.ObterPorIdAsync(dto.Id);
            }
            stopwatch.Stop();

            contatoExcluido.Should().BeNull();

            var processou = await FoiProcessada(queueName, cts.Token);
            processou.Should().BeTrue();

            await consumerService.StopAsync(CancellationToken.None);
            consumerService.Dispose();
        }
    }
}
