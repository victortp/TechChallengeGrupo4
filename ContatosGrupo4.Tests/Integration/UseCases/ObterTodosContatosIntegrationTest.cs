using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Infrastructure.Data.Repositories;
using FluentAssertions;

namespace ContatosGrupo4.Tests.Integration.UseCases
{
    public class ObterTodosContatosIntegrationTest : IClassFixture<SqlServerTests>
    {
        private readonly ObterTodosContatosUseCase _useCase;
        private readonly ContatoRepository _repository;

        public ObterTodosContatosIntegrationTest(SqlServerTests fixture)
        {
            _repository = fixture.contatoRepository;
            _useCase = new ObterTodosContatosUseCase(fixture.contatoRepository);
        }

        [Fact]
        public async Task ExecuteAsync_DeveObterContatosPorDDD()
        {
            var contatos = FakeData.ContatoFake();
            var contato = contatos[0];
            contato.SetDataCriacao();

            await _repository.AdicionarAsync(contato);

            var resultado = await _useCase.ExecuteAsync();

            resultado.Should().NotBeNull();
            resultado.Count().Should().BeGreaterThan(0);
        }
    }
}
