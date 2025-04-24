using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace ContatosGrupo4.Tests.Unit.UseCases.Contatos;

public class ObterContatoPorIdUseCaseTests
{
    private readonly Mock<IContatoRepository> _repository;
    private readonly ObterContatoPorIdUseCase _obterContatoPorIdUseCase;
    public ObterContatoPorIdUseCaseTests()
    {
        var cache = new Mock<IMemoryCache>();

        object unusedCacheValue;
        cache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out unusedCacheValue!))
            .Returns(false);
        cache
            .Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(Mock.Of<ICacheEntry>());

        _repository = new Mock<IContatoRepository>();
        _obterContatoPorIdUseCase = new ObterContatoPorIdUseCase(_repository.Object, cache.Object);

    }

    [Fact]
    public async Task DeveRetornarContatoQuandoIdExistir()
    {
        var contatoEsperado = new Contato() { Id = 1, Nome = "testeContato", Email = "testeemail@google.com", Telefone = "99999-9999" };

        _repository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(contatoEsperado);

        var contato = await _obterContatoPorIdUseCase.ExecuteAsync(1);

        contato.Should().NotBeNull();
        contato!.Id.Should().Be(1);
        contato.Nome.Should().Be(contatoEsperado.Nome);
        contato.Email.Should().Be(contatoEsperado.Email);
        contato.Telefone.Should().Be(contatoEsperado.Telefone);
    }

    [Fact]
    public async Task DeveRetornarNullQuandoIdNaoExistir()
    {
        _repository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(default(Contato));

        var resultado = await _obterContatoPorIdUseCase.ExecuteAsync(1);

        resultado.Should().BeNull();
    }
}