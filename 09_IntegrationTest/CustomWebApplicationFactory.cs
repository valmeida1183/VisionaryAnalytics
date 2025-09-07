using EphemeralMongo;
using Infraestructure.Database;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTest;

public class CustomWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
{
    private readonly IMongoRunner _mongoRunner;

    public CustomWebApplicationFactory()
    {
        _mongoRunner = MongoRunner.Run(new MongoRunnerOptions
        {
            UseSingleNodeReplicaSet = true // Recommended for transactions
        });
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextOptions = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<AppDbContext>));

            if (dbContextOptions != null)
            {
                services.Remove(dbContextOptions);
            }

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMongoDB(_mongoRunner.ConnectionString, "test");
            });

            // Replace MassTransit with in-memory test harness
            var massTransitDescriptors = services
                .Where(d => d.ServiceType.Namespace?.StartsWith("MassTransit") == true)
                .ToList();

            foreach (var descriptor in massTransitDescriptors)
            {
                services.Remove(descriptor);
            }

            services.AddMassTransitTestHarness();
        });
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        _mongoRunner.Dispose();
    }
}
