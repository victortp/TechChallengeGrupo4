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

public class AtualizarContatoUseCaseTests
{
    private readonly Mock<IContatoRepository> _repository;
    private readonly Mock<IMessagePublisher> _messagePublisher;
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
        _obterContatoPorIdUseCase = new ObterContatoPorIdUseCase(_repository.Object, cache.Object);
        _atualizarContatoUseCase = new AtualizarContatoUseCase(
            _obterContatoPorIdUseCase, 
            cache.Object,
            _messagePublisher.Object,
            Options.Create(options));
    }

    [Fact]
    public async Task DeveRetornarFalseQuandoContatoNaoExistir()
    {
        var dto = new AtualizarContatoDto() { Id = 1 };

        _repository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(default(Contato));
        var resultado = await _atualizarContatoUseCase.ExecuteAsync(dto);

        resultado.Should().BeFalse();
    }

    [Theory]
    [InlineData(null, "3299999-9999", "E-mail não informado ou inválido. (Parameter 'Email')")]
    [InlineData("testeemail@google.com", null, "Telefone não informado ou inválido. (Parameter 'Telefone')")]
    public async Task DeveLancarExcecaoQuandoDtoForInvalido(string? email, string? telefone, string mensagem)
    {
        var dto = new AtualizarContatoDto() { Id = 1, Email = email!, Telefone = telefone! };
        var contatoEsperado = new Contato() { Id = dto.Id, Email = dto.Email, Telefone = dto.Telefone };

        _repository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(contatoEsperado);

        var act = async () => { await _atualizarContatoUseCase.ExecuteAsync(dto); };

        await act.Should().ThrowAsync<Exception>().WithMessage(mensagem);
    }

    [Fact]
    public async Task DevePublicarMensagem()
    {
        var dto = new AtualizarContatoDto() { Id = 1, Nome = "contatoTesteAtualizado", Email = "testeemailatualizado@google.com", Telefone = "3299199-9999" };
        var contatoEsperado = new Contato() { Id = dto.Id, Nome = dto.Nome, Email = dto.Email, Telefone = dto.Telefone };

        _repository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(contatoEsperado);

        var resultado = await _atualizarContatoUseCase.ExecuteAsync(dto);

        resultado.Should().BeTrue();
        _messagePublisher.Verify(m => m.PublishAsync<It.IsAnyType>(It.IsAny<It.IsAnyType>(), It.IsAny<string>()), Times.Once);
    }
}