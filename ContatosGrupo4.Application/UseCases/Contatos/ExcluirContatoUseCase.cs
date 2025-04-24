using ContatosGrupo4.Application.Configurations;
using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ContatosGrupo4.Application.UseCases.Contatos;

public class ExcluirContatoUseCase (
    ObterContatoPorIdUseCase contatoPorIdUseCase,
    IMemoryCache memoryCache,
    IMessagePublisher messagePublisher,
    IOptions<RabbitMQOptions> rabbitOptions)
{
    private readonly ObterContatoPorIdUseCase _contatoPorIdUseCase = contatoPorIdUseCase;
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly IMessagePublisher _messagePublisher = messagePublisher;
    private readonly RabbitMQQueues _rabbitQueues = rabbitOptions.Value.Queues;

    public async Task<bool> ExecuteAsync (int idContato)
    {
        var contato = await _contatoPorIdUseCase.ExecuteAsync(idContato);

        if (contato == null) return false;

        await _messagePublisher.PublishAsync(new ExcluirContatoDto { Id = idContato }, _rabbitQueues.ExcluirContato);

        _memoryCache.Remove("TodosContatos");
        _memoryCache.Remove($"Contato_{contato.Id}");
        string ddd = contato.Telefone.Substring(0, 2);
        _memoryCache.Remove($"Contatos_DDD_{ddd}");

        return true;
    }
}