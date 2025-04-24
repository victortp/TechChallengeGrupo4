using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace ContatosGrupo4.Tests.Unit.UseCases.Contatos;

public class CriarContatoUseCaseTests
{
    private readonly Mock<IContatoRepository> _repository;
    private readonly ObterContatoPorNomeEmailUseCase _obterContatoPorNomeEmailUseCase;
    private readonly CriarContatoUseCase _criarContatoUseCase;
    public CriarContatoUseCaseTests()
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
        _obterContatoPorNomeEmailUseCase = new ObterContatoPorNomeEmailUseCase(_repository.Object);
        _criarContatoUseCase = new CriarContatoUseCase(_repository.Object, _obterContatoPorNomeEmailUseCase, cache.Object);
    }

    [Fact]
    public async Task DeveCriarContatoQuandoDtoForValido()
    {

        var dto = new CriarContatoDto() { Nome = "testeContato", Email = "testeemail@google.com", Telefone = "3299999-9999" };
        var contatoEsperado = new Contato { Nome = dto.Nome, Email = dto.Email, Telefone = dto.Telefone };
        _repository
            .Setup(r => r.AdicionarAsync(It.IsAny<Contato>()))
            .Returns(Task.CompletedTask);

        var contatoCriado = await _criarContatoUseCase.ExecuteAsync(dto);

        contatoCriado.Nome.Should().Be(contatoEsperado.Nome);
        contatoCriado.Email.Should().Be(contatoEsperado.Email);
        contatoCriado.Telefone.Should().Be(contatoEsperado.Telefone);
        _repository.Verify(repo => repo.AdicionarAsync(It.Is<Contato>(u =>
            u.Nome == contatoEsperado.Nome &&
            u.Email == contatoEsperado.Email &&
            u.Telefone == contatoEsperado.Telefone)), Times.Once);
    }

    [Theory]
    [InlineData(null, "testeemail@google.com", "3299999-9999", "Nome (Parameter 'O Nome não pode ser vazio.')")]
    [InlineData("testeContato", null, "3299999-9999", "E-mail não informado ou inválido. (Parameter 'Email')")]
    [InlineData("testeContato", "testeemail@google.com", null, "Telefone não informado ou inválido. (Parameter 'Telefone')")]
    public async Task DeveLancarExcecaoQuandoDtoForInvalido(string? nome, string? email, string? telefone, string mensagem)
    {
        var dto = new CriarContatoDto() { Nome = nome!, Email = email!, Telefone = telefone! };

        var act = async () => { await _criarContatoUseCase.ExecuteAsync(dto); };

        await act.Should().ThrowAsync<Exception>().WithMessage(mensagem);
    }

    [Fact]
    public async Task DeveLancarExcecaoQuandoContatoJaExistir()
    {
        var dto = new CriarContatoDto() { Nome = "testeContato", Email = "testeemail@google.com", Telefone = "3299999-9999" };
        var contatoEsperado = new Contato { Nome = dto.Nome, Email = dto.Email, Telefone = dto.Telefone };

        _repository
            .Setup(r => r.ObterPorNomeEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(contatoEsperado);

        var act = async () => { await _criarContatoUseCase.ExecuteAsync(dto); };

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("O Nome/E-mail informados já estão em uso.");
    }

    [Fact]
    public async Task DeveLancarExcecaoQuandoHouverErroDeBanco()
    {
        var dto = new CriarContatoDto() { Nome = "testeContato", Email = "testeemail@google.com", Telefone = "3299999-9999" };

        _repository
            .Setup(r => r.AdicionarAsync(It.IsAny<Contato>()))
            .ThrowsAsync(new Exception());

        var act = async () => { await _criarContatoUseCase.ExecuteAsync(dto); };

        await act.Should().ThrowAsync<Exception>();
    }
}