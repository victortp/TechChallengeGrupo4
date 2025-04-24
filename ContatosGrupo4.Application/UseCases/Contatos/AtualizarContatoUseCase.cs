using ContatosGrupo4.Application.Configurations;
using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.Interfaces;
using ContatosGrupo4.Application.Validations;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ContatosGrupo4.Application.UseCases.Contatos;

public class AtualizarContatoUseCase(
    ObterContatoPorIdUseCase contatoPorIdUseCase,
    IMemoryCache memoryCache,
    IMessagePublisher messagePublisher,
    IOptions<RabbitMQOptions> rabbitOptions)
{
    private readonly ObterContatoPorIdUseCase _contatoPorIdUseCase = contatoPorIdUseCase;
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly IMessagePublisher _messagePublisher = messagePublisher;
    private readonly RabbitMQQueues _rabbitQueues = rabbitOptions.Value.Queues;

    public async Task<bool> ExecuteAsync(AtualizarContatoDto atualizarContatoDto)
    {
        var contato = await _contatoPorIdUseCase.ExecuteAsync(atualizarContatoDto.Id);

        if (contato == null) return false;

        if (!ContatoValidator.ValidarTelefone(atualizarContatoDto.Telefone))
        {
            throw new ArgumentException("Telefone não informado ou inválido.", nameof(atualizarContatoDto.Telefone));
        }

        if (!ContatoValidator.ValidarEmail(atualizarContatoDto.Email))
        {
            throw new ArgumentException("E-mail não informado ou inválido.", nameof(atualizarContatoDto.Email));
        }

        await _messagePublisher.PublishAsync(atualizarContatoDto, _rabbitQueues.AtualizarContato);

        _memoryCache.Remove("TodosContatos");
        _memoryCache.Remove($"Contato_{contato.Id}");
        string ddd = contato.Telefone.Substring(0, 2);
        _memoryCache.Remove($"Contatos_DDD_{ddd}");

        return true;
    }
}