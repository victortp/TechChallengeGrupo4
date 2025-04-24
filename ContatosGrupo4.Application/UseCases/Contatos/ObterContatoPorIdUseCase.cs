using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace ContatosGrupo4.Application.UseCases.Contatos;

public class ObterContatoPorIdUseCase(IContatoRepository contatoRepository, IMemoryCache memoryCache)
{
    private readonly IContatoRepository _contatoRepository = contatoRepository;
    private readonly IMemoryCache _memoryCache = memoryCache;

    public async Task<Contato?> ExecuteAsync(int idContato)
    {
        var cacheKey = $"Contato_{idContato}";

        if (!_memoryCache.TryGetValue(cacheKey, out Contato? contato))
        {
            contato = await _contatoRepository.ObterPorIdAsync(idContato);

            if (contato != null)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };

                _memoryCache.Set(cacheKey, contato, cacheEntryOptions);
            }
        }

        return contato;
    }
}