using CrossCutting.DI;
using QueueConsumer;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services
    .AddInfraestructureConfiguration()
    .AddDataBaseConfiguration(builder.Configuration)
    .AddStorageConfiguration(builder.Configuration)
    .AddMassTransitProducerConfiguration(builder.Configuration);

var host = builder.Build();
host.Run();
