using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using Moq;

namespace ContatosGrupo4.Tests.Application.UseCases.ContatoTests;

public class ExcluirContatoUseCaseTests
{
    [Fact]
    public static async Task DeveExcluirContato()
    {
        var contatoRepository = new Mock<IContatoRepository>();
        var contatoUseCase = new ExcluirContatoUseCase(contatoRepository.Object);
        var contato = new Contato() { Id = 1, Nome = "testeContato", Email = "testeemail@google.com", CodigoArea = 32, Telefone = "99999-9999"};

        contatoRepository.Setup(r => r.DeleteContatos(It.IsAny<int>())).Returns(Task.CompletedTask);

        await contatoUseCase.ExecuteAsync(1);

        contatoRepository.Verify(r => r.DeleteContatos(It.Is<int>(id => id == contato.Id)), Times.Once);
    }
}