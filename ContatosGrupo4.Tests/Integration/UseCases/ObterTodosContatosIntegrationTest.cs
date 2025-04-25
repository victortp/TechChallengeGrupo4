using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Infrastructure.Data.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;

namespace ContatosGrupo4.Tests.Integration.UseCases
{
    public class ObterTodosContatosIntegrationTest : IClassFixture<IntegrationTestsFixture>
    {
        private readonly ObterTodosContatosUseCase _useCase;
        private readonly ContatoRepository _repository;
        private readonly IMemoryCache _memoryCache;

        public ObterTodosContatosIntegrationTest(IntegrationTestsFixture fixture)
        {
            _memoryCache = fixture.MemoryCache;
            _repository = fixture.ContatoRepository;
            _useCase = new ObterTodosContatosUseCase(_repository, _memoryCache);
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
