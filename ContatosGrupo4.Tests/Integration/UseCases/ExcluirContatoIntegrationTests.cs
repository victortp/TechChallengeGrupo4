using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Infrastructure.Data.Repositories;
using FluentAssertions;

namespace ContatosGrupo4.Tests.Integration.UseCases
{
    public class ExcluirContatoIntegrationTests : IClassFixture<SqlServerTests>
    {
        private readonly ExcluirContatoUseCase _useCase;
        private readonly ContatoRepository _repository;

        public ExcluirContatoIntegrationTests(SqlServerTests fixture)
        {
            _repository = fixture.contatoRepository;
            _useCase = new ExcluirContatoUseCase(fixture.contatoRepository);
        }

        [Fact]
        public async Task ExecuteAsync_DeveExcluirContato()
        {
            var contatos = FakeData.ContatoFake();
            var contato = contatos[0];
            contato.SetDataCriacao();

            await _repository.AdicionarAsync(contato);
            var contatoCriado = await _repository.ObterPorNomeEmailAsync(contato.Nome, contato.Email);

            await _useCase.ExecuteAsync(contatoCriado!.Id);

            var resultado = await _repository.ObterPorIdAsync(contatoCriado.Id);

            resultado.Should().BeNull();
        }
    }
}
