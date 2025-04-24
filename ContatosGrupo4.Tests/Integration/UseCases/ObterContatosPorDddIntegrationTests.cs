using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Infrastructure.Data.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;

namespace ContatosGrupo4.Tests.Integration.UseCases
{
    public class ObterContatosPorDddIntegrationTests : IClassFixture<SqlServerTests>
    {
        private readonly ObterContatosPorDddUseCase _useCase;
        private readonly ContatoRepository _repository;
        private readonly IMemoryCache _memoryCache;

        public ObterContatosPorDddIntegrationTests(SqlServerTests fixture)
        {
            _memoryCache = fixture.memoryCache;
            _repository = fixture.contatoRepository;
            _useCase = new ObterContatosPorDddUseCase(_repository, _memoryCache);
        }

        [Fact]
        public async Task ExecuteAsync_DeveObterContatosPorDDD()
        {
            var contatos = FakeData.ContatoFake();
            var contato = contatos[0];
            contato.SetDataCriacao();

            await _repository.AdicionarAsync(contato);

            var resultado = await _useCase.ExecuteAsync((int.Parse(contato.Telefone[..2])));

            resultado.Should().NotBeNull();
            resultado.Count().Should().BeGreaterThan(0);
        }
    }
}
