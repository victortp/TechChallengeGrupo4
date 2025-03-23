using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Infrastructure.Data.Repositories;
using FluentAssertions;

namespace ContatosGrupo4.Tests.Integration.UseCases
{
    public class AtualizarContatoIntegrationTests : IClassFixture<SqlServerTests>
    {
        private readonly AtualizarContatoUseCase _atualizarContatoUseCase;
        private readonly ObterContatoPorIdUseCase _obterContatoPorIdUseCase;
        private readonly ContatoRepository _repository;

        public AtualizarContatoIntegrationTests(SqlServerTests fixture)
        {
            _repository = fixture.contatoRepository;
            _obterContatoPorIdUseCase = new ObterContatoPorIdUseCase(fixture.contatoRepository);
            _atualizarContatoUseCase = new AtualizarContatoUseCase(fixture.contatoRepository, _obterContatoPorIdUseCase);
        }

        [Fact]
        public async Task ExecuteAsync_DeveAtualizarContato()
        {
            var contatos = FakeData.ContatoFake();
            var contato = contatos[0];
            contato.SetDataCriacao();
            
            
            await _repository.AdicionarAsync(contato);
            var contatoCriado = await _repository.ObterPorNomeEmailAsync(contato.Nome, contato.Email);

            var dto = new AtualizarContatoDto
            {
                Id = contatoCriado!.Id,
                Nome = contatos[1].Nome,
                Email = contatos[1].Email,
                Telefone = contatos[1].Telefone
            };

            var resultado = await _atualizarContatoUseCase.ExecuteAsync(dto);

            resultado.Should().NotBeNull();
            resultado.Nome.Should().Be(dto.Nome);
        }
    }
}
