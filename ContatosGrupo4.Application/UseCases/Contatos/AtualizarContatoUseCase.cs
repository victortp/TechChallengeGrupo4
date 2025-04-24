using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.Validations;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace ContatosGrupo4.Application.UseCases.Contatos;

public class AtualizarContatoUseCase(
    IContatoRepository contatoRepository,
    ObterContatoPorIdUseCase contatoPorIdUseCase,
    IMemoryCache memoryCache)
{
    private readonly IContatoRepository _contatoRepository = contatoRepository;
    private readonly ObterContatoPorIdUseCase _contatoPorIdUseCase = contatoPorIdUseCase;
    private readonly IMemoryCache _memoryCache = memoryCache;

    public async Task<Contato?> ExecuteAsync(AtualizarContatoDto atualizarContato)
    {
        var contato = await _contatoPorIdUseCase.ExecuteAsync(atualizarContato.Id);

        if (contato == null) return null;

        if (!ContatoValidator.ValidarTelefone(atualizarContato.Telefone))
        {
            throw new ArgumentException("Telefone não informado ou inválido.", nameof(atualizarContato.Telefone));
        }

        if (!ContatoValidator.ValidarEmail(atualizarContato.Email))
        {
            throw new ArgumentException("E-mail não informado ou inválido.", nameof(atualizarContato.Email));
        }

        contato.Nome = atualizarContato.Nome;
        contato.Telefone = atualizarContato.Telefone;
        contato.Email = atualizarContato.Email;
        contato.SetDataAtualizacao();

        await _contatoRepository.AtualizarAsync(contato);

        _memoryCache.Remove("TodosContatos");
        _memoryCache.Remove($"Contato_{contato.Id}");
        string ddd = contato.Telefone.Substring(0, 2);
        _memoryCache.Remove($"Contatos_DDD_{ddd}");

        return contato;
    }
}