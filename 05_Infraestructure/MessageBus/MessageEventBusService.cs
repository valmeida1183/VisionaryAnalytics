using Application.Abstractions.MessageBus;
using MassTransit;

namespace Infraestructure.MessageBus;
public class MessageEventBusService : IMessageEventBusService
{
    private readonly IBus _bus;

    public MessageEventBusService(IBus bus)
    {
        _bus = bus;
    }

    public async Task PublishAsync<T>(T messageEvent, CancellationToken cancellation = default) where T : class
    {
        await _bus.Publish(messageEvent, cancellation);
    }
}
