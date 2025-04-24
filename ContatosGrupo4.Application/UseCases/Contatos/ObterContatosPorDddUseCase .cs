using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace ContatosGrupo4.Application.UseCases.Contatos;

public class ObterContatosPorDddUseCase(IContatoRepository contatoRepository, IMemoryCache memoryCache)
{
    private readonly IContatoRepository _contatoRepository = contatoRepository;
    private readonly IMemoryCache _memoryCache = memoryCache;

    public async Task<IEnumerable<Contato>> ExecuteAsync(int ddd)
    {
        var cacheKey = $"Contatos_DDD_{ddd}";

        if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Contato>? contatos))
        {
            contatos = await _contatoRepository.ObterPorDddsAsync(ddd);

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };

            _memoryCache.Set(cacheKey, contatos, cacheEntryOptions);
        }

        return contatos ?? [];
    }
}