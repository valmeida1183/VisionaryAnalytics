using FluentValidation;
using Infraestructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Reflection;

namespace CrossCutting.DI;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var appAssembly = Assembly.Load("03_Application");

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(appAssembly)
        );
        services.AddValidatorsFromAssembly(appAssembly, includeInternalTypes: true);

        // Register other application-specific services here
        // e.g., services.AddScoped<IYourService, YourService>();
        return services;
    }

    public static IServiceCollection AddInfraestructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var mongoDbName = configuration.GetValue<string>("MongoDbName") ?? "VisionaryAnalytics";
        var mongoClient = new MongoClient(connectionString);


        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseMongoDB(mongoClient, mongoDbName);
        });

        return services;
    }
}