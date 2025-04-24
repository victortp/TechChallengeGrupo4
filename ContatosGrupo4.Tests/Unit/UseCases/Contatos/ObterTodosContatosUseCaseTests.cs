using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace ContatosGrupo4.Tests.Unit.UseCases.Contatos;

public class ObterTodosContatosUseCaseTests
{
    [Fact]
    public static async Task DeveRetornarTodosContatos()
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
        var contatoUseCase = new ObterTodosContatosUseCase(contatoRepository.Object, cache.Object);
        var contatoEsperado = new Contato() { Nome = "testeContato", Email = "testeemail@google.com", Telefone = "99999-9999" };

        contatoRepository.Setup(r => r.ObterTodosAsync()).ReturnsAsync([contatoEsperado]);

        var contatos = await contatoUseCase.ExecuteAsync();

        contatos.Should().HaveCount(1);
        contatos.First().Nome.Should().Be(contatoEsperado.Nome);
        contatos.First().Email.Should().Be(contatoEsperado.Email);
        contatos.First().Telefone.Should().Be(contatoEsperado.Telefone);
    }
}