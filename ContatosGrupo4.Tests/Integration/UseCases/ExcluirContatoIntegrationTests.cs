using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Infrastructure.Data.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;

namespace ContatosGrupo4.Tests.Integration.UseCases
{
    public class ExcluirContatoIntegrationTests : IClassFixture<SqlServerTests>
    {
        private readonly ExcluirContatoUseCase _useCase;
        private readonly ContatoRepository _repository;
        private readonly IMemoryCache _memoryCache;

        public ExcluirContatoIntegrationTests(SqlServerTests fixture)
        {
            _memoryCache = fixture.memoryCache;
            _repository = fixture.contatoRepository;
            _useCase = new ExcluirContatoUseCase(_repository, _memoryCache);
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
