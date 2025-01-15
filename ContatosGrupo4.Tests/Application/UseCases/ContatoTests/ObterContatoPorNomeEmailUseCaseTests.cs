using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ContatosGrupo4.Tests.Application.UseCases.ContatoTests;

public class ObterContatoPorNomeEmailUseCaseTests
{

    [Fact]
    public static async Task DeveRetornarContatoQuandoNomeEmailExistir()
    {
        var contatoRepository = new Mock<IContatoRepository>();
        var contatoUseCase = new ObterContatoPorNomeEmailUseCase(contatoRepository.Object);
        var contatoEsperado = new Contato() { Id = 1, Nome = "testeContato", Email = "testeemail@google.com", CodigoArea = 32, Telefone = "99999-9999" };

        contatoRepository.Setup(r => r.GetContatoPorNomeEmail(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(contatoEsperado);

        var contato = await contatoUseCase.ExecuteAsync("testeContato", "testeemail@google.com");

        contato.Should().NotBeNull();
        contato!.Id.Should().Be(1);
        contato!.Nome.Should().Be(contatoEsperado.Nome);
        contato!.Email.Should().Be(contatoEsperado.Email);
        contato!.CodigoArea.Should().Be(contatoEsperado.CodigoArea);
        contato!.Telefone.Should().Be(contatoEsperado.Telefone);
    }

    [Fact]
    public static async Task DeveLancarExcecaoQuandoNomeEmailNaoExistir()
    {
        var contatoRepository = new Mock<IContatoRepository>();
        var contatoUseCase = new ObterContatoPorNomeEmailUseCase(contatoRepository.Object);

        contatoRepository.Setup(r => r.GetContatoPorNomeEmail(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(default(Contato));

        var act = async () => await contatoUseCase.ExecuteAsync("testeContato", "testeemail@google.com");

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("Contato com o Nome 'testeContato' e E-mail 'testeemail@google.com' não encontrado.");
    }
}