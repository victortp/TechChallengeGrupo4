using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Application.UseCases.Usuarios;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ContatosGrupo4.Tests.Application.UseCases.Contatos;

public class ObterContatoPorIdUseCaseTests
{
    [Fact]
    public static async Task DeveRetornarContatoQuandoIdExistir()
    {
        var contatoRepository = new Mock<IContatoRepository>();
        var contatoUseCase = new ObterContatoPorIdUseCase(contatoRepository.Object);
        var contatoEsperado = new Contato() { Id = 1, Nome = "testeContato", Email = "testeemail@google.com", Telefone = "99999-9999" };

        contatoRepository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(contatoEsperado);

        var contato = await contatoUseCase.ExecuteAsync(1);

        contato.Should().NotBeNull();
        contato!.Id.Should().Be(1);
        contato!.Nome.Should().Be(contatoEsperado.Nome);
        contato!.Email.Should().Be(contatoEsperado.Email);
        contato!.Telefone.Should().Be(contatoEsperado.Telefone);
    }

    [Fact]
    public static async Task DeveLancarExcecaoQuandoIdNaoExistir()
    {
        var contatoRepository = new Mock<IContatoRepository>();
        var contatoUseCase = new ObterContatoPorIdUseCase(contatoRepository.Object);

        contatoRepository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(default(Contato));

        var act = async () => await contatoUseCase.ExecuteAsync(1);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("Contato com ID 1 não encontrado.");
    }
}