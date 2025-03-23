using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Infrastructure.Data.Repositories;
using FluentAssertions;

namespace ContatosGrupo4.Tests.Integration.UseCases
{
    public class ObterContatoPorNomeEmailIntegrationTests : IClassFixture<SqlServerTests>
    {
        private readonly ObterContatoPorNomeEmailUseCase _useCase;
        private readonly ContatoRepository _repository;

        public ObterContatoPorNomeEmailIntegrationTests(SqlServerTests fixture)
        {
            _repository = fixture.contatoRepository;
            _useCase = new ObterContatoPorNomeEmailUseCase(fixture.contatoRepository);
        }

        [Fact]
        public async Task ExecuteAsync_DeveObterContatoPorNomeEEmail()
        {
            var contatos = FakeData.ContatoFake();
            var contato = contatos[0];
            contato.SetDataCriacao();

            await _repository.AdicionarAsync(contato);

            var resultado = await _useCase.ExecuteAsync(contato.Nome, contato.Email);

            resultado.Should().NotBeNull();
            resultado!.Nome.Should().Be(contato.Nome);
            resultado.Email.Should().Be(contato.Email);
            resultado.Telefone.Should().Be(contato.Telefone);
        }
    }
}
