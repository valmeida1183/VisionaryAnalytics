namespace Application.Abstractions.MessageBus;
public interface IMessageEventBusService
{
    Task PublishAsync<T>(T messageEvent, CancellationToken cancellation = default) where T : class;
}
