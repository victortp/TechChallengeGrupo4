using System.Text;
using System.Text.Json;
using ContatosGrupo4.Application.Configurations;
using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ContatosGrupo4.Infrastructure.Messaging.Consumers
{
    public class ContatoConsumerService : IHostedService, IDisposable
    {
        private readonly ILogger<ContatoConsumerService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConnectionFactory _connectionFactory;
        private readonly RabbitMQQueues _rabbitMQQueues;
        private IConnection? _connection;
        private IChannel? _channel;

        public ContatoConsumerService(
            ILogger<ContatoConsumerService> logger,
            IServiceScopeFactory scopeFactory,
            IConnectionFactory connectionFactory,
            IOptions<RabbitMQOptions> rabbitOptions)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _connectionFactory = connectionFactory;
            _rabbitMQQueues = rabbitOptions.Value.Queues;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando o serviço consumidor.");

            try
            {
                _connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
                _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

                await _channel.QueueDeclareAsync(
                    queue: _rabbitMQQueues.CriarContato,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);
                await _channel.QueueDeclareAsync(
                    queue: _rabbitMQQueues.AtualizarContato,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);
                await _channel.QueueDeclareAsync(
                    queue: _rabbitMQQueues.ExcluirContato,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

                await ConfiguraConsumer(_rabbitMQQueues.CriarContato, ProcessaCriacaoAsync, cancellationToken: cancellationToken);
                await ConfiguraConsumer(_rabbitMQQueues.AtualizarContato, ProcessaAtualizacaoAsync, cancellationToken: cancellationToken);
                await ConfiguraConsumer(_rabbitMQQueues.ExcluirContato, ProcessaExclusaoAsync, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao iniciar o serviço consumidor.");
            }
        }

        private async Task ConfiguraConsumer(
            string queue,
            Func<BasicDeliverEventArgs,
            CancellationToken, Task> handler,
            CancellationToken cancellationToken = default)
        {
            if (_channel == null)
            {
                _logger.LogDebug("Não é possível configurar consumidora para a fila {Queue} porque o channel está nulo.", queue);
                return;
            }

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                _logger.LogDebug("Recebida mensagem da fila {Queue}. DeliveryTag: {DeliveryTag}.", queue, ea.DeliveryTag);
                try
                {
                    await handler(ea, cancellationToken);

                    await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                    _logger.LogDebug("Mensagem da fila {Queue} processada. DeliveryTag: {DeliveryTag}", queue, ea.DeliveryTag);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar mensagem da fila {Queue}. DeliveryTag: {DeliveryTag}.", queue, ea.DeliveryTag);
                    await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: queue,
                autoAck: false,
                consumer: consumer,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Consumidor iniciado para a fila {Queue}.", queue);
        }

        private async Task ProcessaCriacaoAsync(BasicDeliverEventArgs ea, CancellationToken cancellationToken)
        {
            var body = ea.Body.ToArray();
            var jsonMessage = Encoding.UTF8.GetString(body);

            _logger.LogInformation("Processando mensagem de criação: {JsonMessage}.", jsonMessage);

            var dto = JsonSerializer.Deserialize<CriarContatoDto>(jsonMessage)
                ?? throw new JsonException($"Não foi possível deserializar a mensagem para {nameof(CriarContatoDto)}.");

            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IContatoRepository>();

            var contatoExistente = await repository.ObterPorNomeEmailAsync(dto.Nome, dto.Email);
            if (contatoExistente != null)
            {
                _logger.LogWarning("Contato com nome {Nome} e e-mail {Email} já existente. A mensagem será descartada.", dto.Nome, dto.Email);
                return;
            }

            var contato = new Contato
            {
                Nome = dto.Nome,
                Telefone = dto.Telefone,
                Email = dto.Email
            };
            contato.SetDataCriacao();

            await repository.AdicionarAsync(contato);

            _logger.LogInformation("Contato criado com o Id {Id}.", contato.Id);
        }

        private async Task ProcessaAtualizacaoAsync(BasicDeliverEventArgs ea, CancellationToken cancellationToken)
        {
            var body = ea.Body.ToArray();
            var jsonMessage = Encoding.UTF8.GetString(body);

            _logger.LogInformation("Processando mensagem de atualização: {JsonMessage}.", jsonMessage);

            var dto = JsonSerializer.Deserialize<AtualizarContatoDto>(jsonMessage)
                ?? throw new JsonException($"Não foi possível deserializar a mensagem para {nameof(AtualizarContatoDto)}.");

            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IContatoRepository>();

            var contato = await repository.ObterPorIdAsync(dto.Id);
            if (contato == null)
            {
                _logger.LogWarning("Contato com Id {Id} não encontrado. A mensagem será descartada.", dto.Id);
                return;
            }

            contato.Nome = dto.Nome;
            contato.Telefone = dto.Telefone;
            contato.Email = dto.Email;
            contato.SetDataAtualizacao();

            await repository.AtualizarAsync(contato);

            _logger.LogInformation("Contato com o Id {Id} atualizado.", contato.Id);
        }

        private async Task ProcessaExclusaoAsync(BasicDeliverEventArgs ea, CancellationToken cancellationToken)
        {
            var body = ea.Body.ToArray();
            var jsonMessage = Encoding.UTF8.GetString(body);

            _logger.LogInformation("Processando mensagem de exclusão: {JsonMessage}.", jsonMessage);

            var dto = JsonSerializer.Deserialize<ExcluirContatoDto>(jsonMessage)
                ?? throw new JsonException($"Não foi possível deserializar a mensagem para {nameof(ExcluirContatoDto)}.");

            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IContatoRepository>();

            var contato = await repository.ObterPorIdAsync(dto.Id);
            if (contato == null)
            {
                _logger.LogWarning("Contato com Id {Id} não encontrado. A mensagem será descartada.", dto.Id);
                return;
            }

            await repository.ExcluirAsync(dto.Id);

            _logger.LogInformation("Contato com o Id {Id} excluído.", contato.Id);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Parando o serviço consumidor.");
            if (_channel?.IsOpen ?? false) await _channel.CloseAsync(cancellationToken);
            if (_connection?.IsOpen ?? false) await _connection.CloseAsync(cancellationToken);
            _logger.LogInformation("Serviço consumidor parado.");
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
