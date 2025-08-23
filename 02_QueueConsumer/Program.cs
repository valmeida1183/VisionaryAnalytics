using CrossCutting.DI;
using QueueConsumer;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddHostedService<Worker>();

builder.Services
    .AddInfraestructureConfiguration(builder.Configuration)
    .AddDataBaseConfiguration(builder.Configuration)
    .AddMassTransitConsumerConfiguration(builder.Configuration);


var host = builder.Build();
host.Run();
