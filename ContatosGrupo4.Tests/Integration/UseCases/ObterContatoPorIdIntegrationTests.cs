using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Infrastructure.Data.Repositories;
using FluentAssertions;

namespace ContatosGrupo4.Tests.Integration.UseCases
{
    public class ObterContatoPorIdIntegrationTests : IClassFixture<SqlServerTests>
    {
        private readonly ObterContatoPorIdUseCase _useCase;
        private readonly ContatoRepository _repository;

        public ObterContatoPorIdIntegrationTests(SqlServerTests fixture)
        {
            _repository = fixture.contatoRepository;
            _useCase = new ObterContatoPorIdUseCase(fixture.contatoRepository);
        }

        [Fact]
        public async Task ExecuteAsync_DeveObterContatoPorId()
        {
            var contatos = FakeData.ContatoFake();
            var contato = contatos[0];
            contato.SetDataCriacao();

            await _repository.AdicionarAsync(contato);
            var contatoCriado = await _repository.ObterPorNomeEmailAsync(contato.Nome, contato.Email);

            var resultado = await _useCase.ExecuteAsync(contatoCriado!.Id);

            resultado.Should().NotBeNull();
            resultado!.Nome.Should().Be(contato.Nome);
            resultado.Email.Should().Be(contato.Email);
            resultado.Telefone.Should().Be(contato.Telefone);
        }
    }
}