using ContatosGrupo4.Application.Configurations;
using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.UseCases.Contatos;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;

namespace ContatosGrupo4.Tests.Integration.UseCases
{
    public class CriarContatoIntegrationTests : IClassFixture<IntegrationTestsFixture>
    {
        private readonly CriarContatoUseCase _criarContatoUseCase;
        private readonly IMemoryCache _memoryCache;
        private readonly RabbitMQQueues _rabbitMQQueues;
        private readonly IntegrationTestsFixture _fixture;

        public CriarContatoIntegrationTests(IntegrationTestsFixture fixture)
        {
            _fixture = fixture;
            _memoryCache = _fixture.MemoryCache;
            _rabbitMQQueues = _fixture.RabbitMqOptions.Value.Queues;
            var obterContatoPorNomeEmailUseCase = new ObterContatoPorNomeEmailUseCase(fixture.ContatoRepository);
            _criarContatoUseCase = new CriarContatoUseCase(
                obterContatoPorNomeEmailUseCase, 
                _memoryCache,
                _fixture.RabbitMQPublisher,
                _fixture.RabbitMqOptions);
        }

        [Fact]
        public async Task ExecuteAsync_DevePublicarMensagem()
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

            await _criarContatoUseCase.ExecuteAsync(dto);

            var mensagemPublicada = await _fixture.ConsumeMessageAsync<CriarContatoDto>(_rabbitMQQueues.CriarContato, TimeSpan.FromSeconds(5));
            mensagemPublicada.Should().NotBeNull();
            mensagemPublicada!.Nome.Should().Be(dto.Nome);
        }
    }
}
