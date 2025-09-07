using WebApi.Extensions;
using CrossCutting.DI;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    // Remove the file size limitation for Kestrel
    options.Limits.MaxRequestBodySize = null; 
});

builder.Services
    .AddWebApiConfiguration()
    .AddApplicationConfiguration()
    .AddInfraestructureConfiguration(builder.Configuration)
    .AddDataBaseConfiguration(builder.Configuration)
    .AddMassTransitProducerConfiguration(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.AddEndpoints();

app.Run();

public partial class Program { }
