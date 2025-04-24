using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Infrastructure.Data.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;

namespace ContatosGrupo4.Tests.Integration.UseCases
{
    public class AtualizarContatoIntegrationTests : IClassFixture<SqlServerTests>
    {
        private readonly AtualizarContatoUseCase _atualizarContatoUseCase;
        private readonly ObterContatoPorIdUseCase _obterContatoPorIdUseCase;
        private readonly ContatoRepository _repository;
        private readonly IMemoryCache _memoryCache;

        public AtualizarContatoIntegrationTests(SqlServerTests fixture)
        {
            _memoryCache = fixture.memoryCache;
            _repository = fixture.contatoRepository;
            _obterContatoPorIdUseCase = new ObterContatoPorIdUseCase(_repository, _memoryCache);
            _atualizarContatoUseCase = new AtualizarContatoUseCase(_repository, _obterContatoPorIdUseCase, _memoryCache);
        }

        [Fact]
        public async Task ExecuteAsync_DeveAtualizarContato()
        {
            var contatos = FakeData.ContatoFake();
            var contato = contatos[0];
            contato.SetDataCriacao();
            
            
            await _repository.AdicionarAsync(contato);
            var contatoCriado = await _repository.ObterPorNomeEmailAsync(contato.Nome, contato.Email);
            contatoCriado.Should().NotBeNull();
            int contatoId = contatoCriado!.Id;

            var dto = new AtualizarContatoDto
            {
                Id = contatoId,
                Nome = contatos[1].Nome,
                Email = contatos[1].Email,
                Telefone = contatos[1].Telefone
            };

            var resultado = await _atualizarContatoUseCase.ExecuteAsync(dto);

            resultado.Should().NotBeNull();
            resultado!.Nome.Should().Be(dto.Nome);
        }

        [Fact]
        public async Task ExecuteAsync_DeveRetornarNull_QuandoContatoNaoExiste()
        {
            var dto = new AtualizarContatoDto
            {
                Id = 999999,
                Nome = "Teste Nao Existe",
                Email = "nao@existe.com",
                Telefone = "11999998888"
            };

            var resultado = await _atualizarContatoUseCase.ExecuteAsync(dto);

            resultado.Should().BeNull();
        }
    }
}
