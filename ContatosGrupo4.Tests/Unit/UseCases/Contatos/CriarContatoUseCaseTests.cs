using ContatosGrupo4.Application.Configurations;
using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.Interfaces;
using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;

namespace ContatosGrupo4.Tests.Unit.UseCases.Contatos;

public class CriarContatoUseCaseTests
{
    private readonly Mock<IContatoRepository> _repository;
    private readonly Mock<IMessagePublisher> _messagePublisher;
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

        var options = new RabbitMQOptions
        {
            HostName = string.Empty,
            UserName = string.Empty,
            Password = string.Empty,
            Queues = new RabbitMQQueues
            {
                CriarContato = "contato-criar-queue",
                AtualizarContato = "contato-atualizar-queue",
                ExcluirContato = "contato-excluir-queue"
            }
        };

        _repository = new Mock<IContatoRepository>();
        _messagePublisher = new Mock<IMessagePublisher>();
        _obterContatoPorNomeEmailUseCase = new ObterContatoPorNomeEmailUseCase(_repository.Object);
        _criarContatoUseCase = new CriarContatoUseCase(
            _obterContatoPorNomeEmailUseCase,
            cache.Object,
            _messagePublisher.Object,
            Options.Create(options));
    }

    [Fact]
    public async Task DevePublicarMensagemQuandoDtoForValido()
    {

        var dto = new CriarContatoDto() { Nome = "testeContato", Email = "testeemail@google.com", Telefone = "3299999-9999" };
        var contatoEsperado = new Contato { Nome = dto.Nome, Email = dto.Email, Telefone = dto.Telefone };
        _repository
            .Setup(r => r.AdicionarAsync(It.IsAny<Contato>()))
            .Returns(Task.CompletedTask);

        await _criarContatoUseCase.ExecuteAsync(dto);

        _messagePublisher.Verify(m => m.PublishAsync<It.IsAnyType>(It.IsAny<It.IsAnyType>(), It.IsAny<string>()), Times.Once);
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
    public async Task DeveLancarExcecaoQuandoHouverErroNoMessageBroker()
    {
        var dto = new CriarContatoDto() { Nome = "testeContato", Email = "testeemail@google.com", Telefone = "3299999-9999" };

        _messagePublisher
            .Setup(m => m.PublishAsync<It.IsAnyType>(It.IsAny<It.IsAnyType>(), It.IsAny<string>()))
            .ThrowsAsync(new ApplicationException());

        var act = async () => { await _criarContatoUseCase.ExecuteAsync(dto); };

        await act.Should().ThrowAsync<ApplicationException>();
    }
}