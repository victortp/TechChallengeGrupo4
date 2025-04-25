using ContatosGrupo4.Application.Configurations;
using ContatosGrupo4.Application.Interfaces;
using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;

namespace ContatosGrupo4.Tests.Unit.UseCases.Contatos;

public class ExcluirContatoUseCaseTests
{
    [Fact]
    public static async Task DeveExcluirContato()
    {
        var cache = new Mock<IMemoryCache>();

        object unusedCacheValue;
        cache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out unusedCacheValue!))
            .Returns(false);
        cache
            .Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(Mock.Of<ICacheEntry>());

        var options = new RabbitMQOptions
        {
            HostName = string.Empty,
            UserName = string.Empty,
            Password = string.Empty,
            Queues = new RabbitMQQueues
            {
                CriarContato = "contato-criar-queue",
                AtualizarContato = "contato-atualizar-queue",
                ExcluirContato = "contato-excluir-queue"
            }
        };

        var contatoRepository = new Mock<IContatoRepository>();
        var messagePublisher = new Mock<IMessagePublisher>();
        var obterContatoPorIdUseCase = new ObterContatoPorIdUseCase(contatoRepository.Object, cache.Object);
        var contatoUseCase = new ExcluirContatoUseCase(
            obterContatoPorIdUseCase,
            cache.Object,
            messagePublisher.Object,
            Options.Create(options));
        var contato = new Contato() { Id = 1, Nome = "testeContato", Email = "testeemail@google.com", Telefone = "99999-9999" };

        contatoRepository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(contato);
        
        await contatoUseCase.ExecuteAsync(1);

        messagePublisher.Verify(m => m.PublishAsync<It.IsAnyType>(It.IsAny<It.IsAnyType>(), It.IsAny<string>()), Times.Once);
    }
}