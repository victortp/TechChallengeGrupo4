using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.UseCases.Contatos;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;

namespace ContatosGrupo4.Tests.Integration.UseCases
{
    public class CriarContatoIntegrationTests : IClassFixture<SqlServerTests>
    {
        private readonly CriarContatoUseCase _criarContatoUseCase;
        private readonly IMemoryCache _memoryCache;

        public CriarContatoIntegrationTests(SqlServerTests fixture)
        {
            _memoryCache = fixture.memoryCache;
            var obterContatoPorNomeEmailUseCase = new ObterContatoPorNomeEmailUseCase(fixture.contatoRepository);
            _criarContatoUseCase = new CriarContatoUseCase(fixture.contatoRepository, obterContatoPorNomeEmailUseCase, _memoryCache);
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
