using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Domain.Interfaces;
using ContatosGrupo4.Domain.Entities;
using Moq;
using FluentAssertions;

namespace ContatosGrupo4.Tests.Application.UseCases.ContatoTests;

public class CriarContatoUseCaseTests
{
    [Fact]
    public static async Task DeveCriarContatoQuandoDtoForValido()
    {
        var contatoRepository = new Mock<IContatoRepository>();
        var obterContatoPorNomeEmail = new Mock<ObterContatoPorNomeEmailUseCase>();
        var contatoUseCase = new CriarContatoUseCase(contatoRepository.Object, obterContatoPorNomeEmail.Object);
        var dto = new CriarContatoDto() { Nome = "testeContato", Email = "testeemail@google.com", CodigoArea = 32, Telefone = "99999-9999" };
        var contatoEsperado = new Contato { Nome = "testeContato", Email = "testeemail@google.com", CodigoArea = 32, Telefone = "99999-9999" };
        contatoRepository
            .Setup(r => r.PostContatos(It.IsAny<Contato>()))
            .Returns(Task.CompletedTask);
        
        var contatoCriado = await contatoUseCase.ExecuteAsync(dto);

        contatoCriado.Nome.Should().Be(contatoEsperado.Nome);
        contatoCriado.Email.Should().Be(contatoEsperado.Email);
        contatoCriado.CodigoArea.Should().Be(contatoEsperado.CodigoArea);
        contatoCriado.Telefone.Should().Be(contatoEsperado.Telefone);
        contatoRepository.Verify(repo => repo.PostContatos(It.Is<Contato>(u =>
            u.Nome == "testeContato" &&
            u.Email == "testeemail@google.com" &&
            u.CodigoArea == 32 &&
            u.Telefone == "99999-9999")), Times.Once);
    }
    
    [Theory]
    [InlineData(null, "testeemail@google.com", 32, "99999-9999", "Nome (Parameter 'O Nome não pode ser vazio.')")]
    [InlineData("testeContato", null, 32, "99999-9999", "Email (Parameter 'O E-mail não pode ser vazio.')")]
    [InlineData("testeContato", "testeemail@google.com", 0, "99999-9999", "CodigoArea (Parameter 'O Código da Área não pode ser igual a zero.')")]
    [InlineData("testeContato", "testeemail@google.com", 32, null, "Telefone (Parameter 'O Telefone não pode ser vazio.')")]
    public static async Task DeveLancarExcecaoQuandoDtoForInvalido(string? nome, string? email, int codigoArea, string? telefone, string mensagem)
    {
        var contatoRepository = new Mock<IContatoRepository>();
        var obterContatoPorNomeEmail = new Mock<ObterContatoPorNomeEmailUseCase>();
        var contatoUseCase = new CriarContatoUseCase(contatoRepository.Object, obterContatoPorNomeEmail.Object);
        var dto = new CriarContatoDto() { Nome = nome!, Email = email!, CodigoArea = codigoArea, Telefone = telefone!};
        var act = async () => { await contatoUseCase.ExecuteAsync(dto); };

        await act.Should().ThrowAsync<ArgumentNullException>().WithMessage(mensagem);
    }

    [Fact]
    public static async Task DeveLancarExcecaoQuandoContatoJaExistir()
    {
        var contatoRepository = new Mock<IContatoRepository>();
        var obterContatoPorNomeEmail = new Mock<ObterContatoPorNomeEmailUseCase>();
        var contatoUseCase = new CriarContatoUseCase(contatoRepository.Object, obterContatoPorNomeEmail.Object);
        var dto = new CriarContatoDto() { Nome = "testeContato", Email = "testeemail@google.com", CodigoArea = 32, Telefone = "99999-9999"};
        var contatoEsperado = new Contato() { Nome = "testeContato", Email = "testeemail@google.com", CodigoArea = 32, Telefone = "99999-9999" };

        contatoRepository
            .Setup(r => r.GetContatoPorNomeEmail(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(contatoEsperado);

        var act = async () => { await contatoUseCase.ExecuteAsync(dto); };

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("O Nome/E-mail informados já estão em uso.");
    }

    [Fact]
    public static async Task DeveLancarExcecaoQuandoHouverErroDeBanco()
    {
        var contatoRepository = new Mock<IContatoRepository>();
        var obterContatoPorNomeEmail = new Mock<ObterContatoPorNomeEmailUseCase>();
        var contatoUseCase = new CriarContatoUseCase(contatoRepository.Object, obterContatoPorNomeEmail.Object);
        var dto = new CriarContatoDto() { Nome = "testeContato", Email = "testeemail@google.com", CodigoArea = 32, Telefone = "99999-9999" };
        var contatoEsperado = new Contato() { Nome = "testeContato", Email = "testeemail@google.com", CodigoArea = 32, Telefone = "99999-9999" };

        contatoRepository
            .Setup(r => r.PostContatos(It.IsAny<Contato>()))
            .ThrowsAsync(new Exception());

        var act = async () => { await contatoUseCase.ExecuteAsync(dto); };

        await act.Should().ThrowAsync<Exception>();
    }
}