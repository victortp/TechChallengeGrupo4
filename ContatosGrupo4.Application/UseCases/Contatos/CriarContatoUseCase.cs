using ContatosGrupo4.Application.Configurations;
using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.Interfaces;
using ContatosGrupo4.Application.Validations;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ContatosGrupo4.Application.UseCases.Contatos;

public class CriarContatoUseCase(
    ObterContatoPorNomeEmailUseCase obterContatoPorNomeEmail,
    IMemoryCache memoryCache,
    IMessagePublisher messagePublisher,
    IOptions<RabbitMQOptions> rabbitOptions)
{
    private readonly ObterContatoPorNomeEmailUseCase _obterContatoPorNomeEmail = obterContatoPorNomeEmail;
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly IMessagePublisher _messagePublisher = messagePublisher;
    private readonly RabbitMQQueues _rabbitQueues = rabbitOptions.Value.Queues;

    public async Task ExecuteAsync(CriarContatoDto contatoDto)
    {
        if (string.IsNullOrEmpty(contatoDto.Nome))
        {
            throw new ArgumentNullException("O Nome não pode ser vazio.", nameof(contatoDto.Nome));
        }

        if (!ContatoValidator.ValidarTelefone(contatoDto.Telefone))
        {
            throw new ArgumentException("Telefone não informado ou inválido.", nameof(contatoDto.Telefone));
        }

        if (!ContatoValidator.ValidarEmail(contatoDto.Email))
        {
            throw new ArgumentException("E-mail não informado ou inválido.", nameof(contatoDto.Email));
        }

        var contatoExistente = await _obterContatoPorNomeEmail.ExecuteAsync(contatoDto.Nome, contatoDto.Email);
        if (contatoExistente != null)
        {
            throw new InvalidOperationException("O Nome/E-mail informados já estão em uso.");
        }

        var contato = new Contato()
        {
            Nome = contatoDto.Nome,
            Telefone = contatoDto.Telefone,
            Email = contatoDto.Email
        };
        contato.SetDataCriacao();

        await _messagePublisher.PublishAsync(contato, _rabbitQueues.CriarContato);

        _memoryCache.Remove("TodosContatos");
        string ddd = contato.Telefone.Substring(0, 2);
        _memoryCache.Remove($"Contatos_DDD_{ddd}");
    }
}