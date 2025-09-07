using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Reflection;

namespace IntegrationTest;

public class ConsumerTestWebApplicationFactory<TEntryPoint> : CustomWebApplicationFactory<TEntryPoint> where TEntryPoint : class
{
    public readonly Mock<ISender> SenderMock = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {
            // Replace MediatR with a mock
            var senderDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ISender));
            if (senderDescriptor != null)
            {
                services.Remove(senderDescriptor);
            }
            services.AddSingleton(SenderMock.Object);

            // Add consumers to the test harness
            var massTransitDescriptors = services.Where(d => d.ServiceType.Namespace?.StartsWith("MassTransit") == true).ToList();
            foreach (var descriptor in massTransitDescriptors)
            {
                services.Remove(descriptor);
            }

            services.AddMassTransitTestHarness(cfg =>
            {
                var assembly = Assembly.Load("02_QueueConsumer");
                cfg.AddConsumers(assembly);
            });
        });
    }
}
