using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace ContatosGrupo4.Tests.Unit.UseCases.Contatos;

public class ExcluirContatoUseCaseTests
{
    [Fact]
    public static async Task DeveExcluirContato()
    {
        var cache = new Mock<IMemoryCache>();

        object unusedCacheValue;
        cache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out unusedCacheValue!))
            .Returns(false);
        cache
            .Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(Mock.Of<ICacheEntry>());

        var contatoRepository = new Mock<IContatoRepository>();
        var contatoUseCase = new ExcluirContatoUseCase(contatoRepository.Object, cache.Object);
        var contato = new Contato() { Id = 1, Nome = "testeContato", Email = "testeemail@google.com", Telefone = "99999-9999"};

        contatoRepository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(contato);
        contatoRepository.Setup(r => r.ExcluirAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

        await contatoUseCase.ExecuteAsync(1);

        contatoRepository.Verify(r => r.ExcluirAsync(It.Is<int>(id => id == contato.Id)), Times.Once);
    }
}