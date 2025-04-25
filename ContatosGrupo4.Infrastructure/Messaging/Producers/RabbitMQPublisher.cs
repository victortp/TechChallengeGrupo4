using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using ContatosGrupo4.Application.Interfaces;
using RabbitMQ.Client;

namespace ContatosGrupo4.Infrastructure.Messaging.Producers
{
    [ExcludeFromCodeCoverage]
    public class RabbitMQPublisher(IConnectionFactory connectionFactory) : IMessagePublisher
    {
        private readonly IConnectionFactory _connectionFactory = connectionFactory;

        public async Task PublishAsync<T>(T message, string queueName)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                var jsonMessage = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(jsonMessage);

                await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: queueName,
                    mandatory: true,
                    basicProperties: new BasicProperties { Persistent = true },
                    body: body);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Falha ao publicar mensagem para a fila {queueName}", ex);
            }
        }
    }
}
