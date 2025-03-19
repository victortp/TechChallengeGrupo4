using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ContatosGrupo4.Tests.Application.UseCases.Contatos
{
    public class ObterContatosPorDddUseCaseTests
    {
        [Fact]
        public static async Task DeveRetornarTodosContatosComDdd()
        {
            var contatoRepository = new Mock<IContatoRepository>();
            var contatoUseCase = new ObterContatosPorDddUseCase(contatoRepository.Object);
            var contatoEsperado = new Contato() { Nome = "testeContato", Email = "testeemail@google.com", Telefone = "3299999-9999" };

            contatoRepository.Setup(r => r.ObterPorDddsAsync(It.IsAny<int>())).ReturnsAsync([contatoEsperado]);

            var contatos = await contatoUseCase.ExecuteAsync(32);

            contatos.Should().HaveCount(1);
            contatos.First().Nome.Should().Be(contatoEsperado.Nome);
            contatos.First().Email.Should().Be(contatoEsperado.Email);
            contatos.First().Telefone.Should().Be(contatoEsperado.Telefone);
        }
    }
}
