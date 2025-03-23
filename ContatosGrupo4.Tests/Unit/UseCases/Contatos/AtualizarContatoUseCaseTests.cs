using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ContatosGrupo4.Tests.Unit.UseCases.Contatos;

public class AtualizarContatoUseCaseTests
{
    [Fact]
    public static async Task DeveLancarExcecaoQuandoContatoNaoExistir()
    {
        var contatoRepository = new Mock<IContatoRepository>();
        var obterContatoPorIdUSeCase = new ObterContatoPorIdUseCase(contatoRepository.Object);
        var contatoUseCase = new AtualizarContatoUseCase(contatoRepository.Object, obterContatoPorIdUSeCase);
        var dto = new AtualizarContatoDto() { Id = 1 };

        contatoRepository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(default(Contato));
        var act = async () => await contatoUseCase.ExecuteAsync(dto);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("Contato com ID 1 não encontrado.");
    }

    [Fact]
    public static async Task DeveAtualizarERetornarContato()
    {
        var contatoRepository = new Mock<IContatoRepository>();
        var obterContatoPorIdUSeCase = new ObterContatoPorIdUseCase(contatoRepository.Object);
        var contatoUseCase = new AtualizarContatoUseCase(contatoRepository.Object, obterContatoPorIdUSeCase);
        var dto = new AtualizarContatoDto() { Id = 1, Nome = "contatoTesteAtualizado", Email = "testeemailatualizado@google.com", Telefone = "3299199-9999" };
        var contatoEsperado = new Contato() { Id = dto.Id, Nome = dto.Nome, Email = dto.Email, Telefone = dto.Telefone };

        contatoRepository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(contatoEsperado);
        contatoRepository.Setup(r => r.AtualizarAsync(It.IsAny<Contato>())).Returns(Task.CompletedTask);

        var contato = await contatoUseCase.ExecuteAsync(dto);

        contato.Id.Should().Be(1);
        contato.Nome.Should().Be(contatoEsperado.Nome);
        contato.Email.Should().Be(contatoEsperado.Email);
        contato.Telefone.Should().Be(contatoEsperado.Telefone);
        
        contatoRepository.Verify(r => r.AtualizarAsync(It.Is<Contato>(u =>
            u.Id == dto.Id &&
            u.Nome == dto.Nome &&
            u.Email == dto.Email &&
            u.Telefone == dto.Telefone)), Times.Once);
    }
}