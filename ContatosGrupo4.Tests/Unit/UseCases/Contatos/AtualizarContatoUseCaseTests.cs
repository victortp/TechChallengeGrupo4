using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace ContatosGrupo4.Tests.Unit.UseCases.Contatos;

public class AtualizarContatoUseCaseTests
{
    private readonly Mock<IContatoRepository> _repository;
    private readonly ObterContatoPorIdUseCase _obterContatoPorIdUseCase;
    private readonly AtualizarContatoUseCase _atualizarContatoUseCase;

    public AtualizarContatoUseCaseTests()
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
        _atualizarContatoUseCase = new AtualizarContatoUseCase(_repository.Object, _obterContatoPorIdUseCase, cache.Object);
    }

    [Fact]
    public async Task DeveRetornarNullQuandoContatoNaoExistir()
    {
        var dto = new AtualizarContatoDto() { Id = 1 };

        _repository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(default(Contato));
        var resultado = await _atualizarContatoUseCase.ExecuteAsync(dto);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task DeveAtualizarERetornarContato()
    {
        var dto = new AtualizarContatoDto() { Id = 1, Nome = "contatoTesteAtualizado", Email = "testeemailatualizado@google.com", Telefone = "3299199-9999" };
        var contatoEsperado = new Contato() { Id = dto.Id, Nome = dto.Nome, Email = dto.Email, Telefone = dto.Telefone };

        _repository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(contatoEsperado);
        _repository.Setup(r => r.AtualizarAsync(It.IsAny<Contato>())).Returns(Task.CompletedTask);

        var contato = await _atualizarContatoUseCase.ExecuteAsync(dto);

        contato!.Id.Should().Be(1);
        contato.Nome.Should().Be(contatoEsperado.Nome);
        contato.Email.Should().Be(contatoEsperado.Email);
        contato.Telefone.Should().Be(contatoEsperado.Telefone);

        _repository.Verify(r => r.AtualizarAsync(It.Is<Contato>(u =>
            u.Id == dto.Id &&
            u.Nome == dto.Nome &&
            u.Email == dto.Email &&
            u.Telefone == dto.Telefone)), Times.Once);
    }
}