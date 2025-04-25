using ContatosGrupo4.Application.Configurations;
using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Infrastructure.Data.Repositories;
using FluentAssertions;

namespace ContatosGrupo4.Tests.Integration.UseCases
{
    public class AtualizarContatoIntegrationTests : IClassFixture<IntegrationTestsFixture>
    {
        private readonly AtualizarContatoUseCase _atualizarContatoUseCase;
        private readonly ObterContatoPorIdUseCase _obterContatoPorIdUseCase;
        private readonly ContatoRepository _repository;
        private readonly RabbitMQQueues _rabbitMQQueues;
        private readonly IntegrationTestsFixture _fixture;

        public AtualizarContatoIntegrationTests(IntegrationTestsFixture fixture)
        {
            _fixture = fixture;
            var memoryCache = fixture.MemoryCache;
            _repository = _fixture.ContatoRepository;
            _rabbitMQQueues = _fixture.RabbitMqOptions.Value.Queues;
            _obterContatoPorIdUseCase = new ObterContatoPorIdUseCase(_repository, memoryCache);
            _atualizarContatoUseCase = new AtualizarContatoUseCase(
                _obterContatoPorIdUseCase,
                memoryCache,
                _fixture.RabbitMQPublisher,
                _fixture.RabbitMqOptions);
        }

        [Fact]
        public async Task ExecuteAsync_DevePublicarMensagem()
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

            resultado.Should().BeTrue();

            var mensagemPublicada = await _fixture.ConsumeMessageAsync<AtualizarContatoDto>(_rabbitMQQueues.AtualizarContato, TimeSpan.FromSeconds(5));

            mensagemPublicada.Should().NotBeNull();
            mensagemPublicada!.Id.Should().Be(contatoId);
        }

        [Fact]
        public async Task ExecuteAsync_DeveRetornarFalse_QuandoContatoNaoExiste()
        {
            var dto = new AtualizarContatoDto
            {
                Id = 999999,
                Nome = "Teste Nao Existe",
                Email = "nao@existe.com",
                Telefone = "11999998888"
            };

            var resultado = await _atualizarContatoUseCase.ExecuteAsync(dto);

            resultado.Should().BeFalse();
        }
    }
}
