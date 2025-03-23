using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.UseCases.Contatos;
using FluentAssertions;

namespace ContatosGrupo4.Tests.Integration.UseCases
{
    public class CriarContatoIntegrationTests : IClassFixture<SqlServerTests>
    {
        private readonly CriarContatoUseCase _criarContatoUseCase;

        public CriarContatoIntegrationTests(SqlServerTests fixture)
        {
            var obterContatoPorNomeEmailUseCase = new ObterContatoPorNomeEmailUseCase(fixture.contatoRepository);
            _criarContatoUseCase = new CriarContatoUseCase(fixture.contatoRepository, obterContatoPorNomeEmailUseCase);
        }

        [Fact]
        public async Task ExecuteAsync_DeveCriarContato()
        {
            var contatos = FakeData.ContatoFake();
            var contato = contatos[0];
            contato.SetDataCriacao();

            var dto = new CriarContatoDto
            {
                Nome = contato.Nome,
                Email = contato.Email,
                Telefone = contato.Telefone
            };

            var resultado = await _criarContatoUseCase.ExecuteAsync(dto);
            
            resultado.Nome.Should().Be(contato.Nome);
            resultado.Email.Should().Be(contato.Email);
            resultado.Telefone.Should().Be(contato.Telefone);
        }
    }
}
