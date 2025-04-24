namespace ContatosGrupo4.Application.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message, string queue);
    }
}
