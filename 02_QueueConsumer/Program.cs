using CrossCutting.DI;
using QueueConsumer;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddHostedService<Worker>();

builder.Services
    .AddInfraestructureConfiguration()
    .AddDataBaseConfiguration(builder.Configuration)
    .AddStorageConfiguration(builder.Configuration)
    .AddMassTransitConsumerConfiguration(builder.Configuration);


var host = builder.Build();
host.Run();
