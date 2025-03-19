using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ContatosGrupo4.Tests.Unit.UseCases.Contatos;

public class ObterContatoPorNomeEmailUseCaseTests
{
    [Fact]
    public static async Task DeveRetornarContatoQuandoNomeEmailExistir()
    {
        var contatoRepository = new Mock<IContatoRepository>();
        var contatoUseCase = new ObterContatoPorNomeEmailUseCase(contatoRepository.Object);
        var contatoEsperado = new Contato() { Id = 1, Nome = "testeContato", Email = "testeemail@google.com", Telefone = "3299999-9999" };

        contatoRepository.Setup(r => r.ObterPorNomeEmailAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(contatoEsperado);

        var contato = await contatoUseCase.ExecuteAsync("testeContato", "testeemail@google.com");

        contato.Should().NotBeNull();
        contato!.Id.Should().Be(1);
        contato!.Nome.Should().Be(contatoEsperado.Nome);
        contato!.Email.Should().Be(contatoEsperado.Email);
        contato!.Telefone.Should().Be(contatoEsperado.Telefone);
    }
}