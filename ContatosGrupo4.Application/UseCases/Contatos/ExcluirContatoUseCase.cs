using ContatosGrupo4.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace ContatosGrupo4.Application.UseCases.Contatos;

public class ExcluirContatoUseCase (IContatoRepository contatoRepository, IMemoryCache memoryCache)
{
    private readonly IContatoRepository _contatoRepository = contatoRepository;
    private readonly IMemoryCache _memoryCache = memoryCache;

    public async Task ExecuteAsync (int idContato)
    {
        var contato = await _contatoRepository.ObterPorIdAsync(idContato);

        if (contato == null) return;

        await _contatoRepository.ExcluirAsync(idContato);

        _memoryCache.Remove("TodosContatos");
        _memoryCache.Remove($"Contato_{contato.Id}");
        string ddd = contato.Telefone.Substring(0, 2);
        _memoryCache.Remove($"Contatos_DDD_{ddd}");
    }
}