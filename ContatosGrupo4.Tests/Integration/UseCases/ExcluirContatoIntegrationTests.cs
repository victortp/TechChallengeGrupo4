using ContatosGrupo4.Application.Configurations;
using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Infrastructure.Data.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;

namespace ContatosGrupo4.Tests.Integration.UseCases
{
    public class ExcluirContatoIntegrationTests : IClassFixture<IntegrationTestsFixture>
    {
        private readonly ObterContatoPorIdUseCase _obterContatoPorIdUseCase;
        private readonly ExcluirContatoUseCase _useCase;
        private readonly ContatoRepository _repository;
        private readonly IMemoryCache _memoryCache;
        private readonly RabbitMQQueues _rabbitMQQueues;
        private readonly IntegrationTestsFixture _fixture;

        public ExcluirContatoIntegrationTests(IntegrationTestsFixture fixture)
        {
            _fixture = fixture;
            _rabbitMQQueues = _fixture.RabbitMqOptions.Value.Queues;
            _memoryCache = fixture.MemoryCache;
            _repository = fixture.ContatoRepository;
            _obterContatoPorIdUseCase = new ObterContatoPorIdUseCase(_repository, _memoryCache);
            _useCase = new ExcluirContatoUseCase(
                _obterContatoPorIdUseCase, 
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

            await _repository.AdicionarAsync(contato);
            var contatoCriado = await _repository.ObterPorNomeEmailAsync(contato.Nome, contato.Email);

            contatoCriado.Should().NotBeNull();

            var resultado = await _useCase.ExecuteAsync(contatoCriado!.Id);

            resultado.Should().BeTrue();

            var mensagemPublicada = await _fixture.ConsumeMessageAsync<ExcluirContatoDto>(_rabbitMQQueues.ExcluirContato, TimeSpan.FromSeconds(5));
            mensagemPublicada.Should().NotBeNull();
            mensagemPublicada!.Id.Should().Be(contato.Id);
        }
    }
}
