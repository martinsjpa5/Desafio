
namespace Domain.Interfaces.Queues
{
    public interface IRabbitMqQueue<TMessage>
    {
        Task PublishMessageAsync(TMessage message);
        void StartConsumers(int consumerCount, Action<TMessage> processMessage, CancellationToken cancellationToken);
    }
}
