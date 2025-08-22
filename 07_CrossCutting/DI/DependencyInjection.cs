using Application.Abstractions.Repositories;
using Application.Abstractions.Storage;
using FluentValidation;
using Infraestructure.Database;
using Infraestructure.Database.Repositories;
using Infraestructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Reflection;
using MassTransit;
using Application.Abstractions.MessageBus;
using Infraestructure.MessageBus;

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
        services
            .AddDataBaseConfiguration(configuration)
            .AddStorageConfiguration(configuration)
            .AddMassTransitConfiguration(configuration);

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IVideoProcessResultRepository, VideoProcessResultRepository>();
        services.AddScoped<IVideoQrCodeRepository, VideoQrCodeRepository>();

        return services;
    }

    private static IServiceCollection AddDataBaseConfiguration(this IServiceCollection services, IConfiguration configuration)
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

    private static IServiceCollection AddMassTransitConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var host = configuration.GetSection("RabbitMq:Host").Value;
        var user = configuration.GetSection("RabbitMq:User").Value ?? "guest";
        var password = configuration.GetSection("RabbitMq:Password").Value ?? "guest";

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(host, "/", h =>
                {
                    h.Username(user);
                    h.Password(password);
                });
            });
        });

        services.AddScoped<IMessageEventBusService, MessageEventBusService>();

        return services;
    }

    private static IServiceCollection AddStorageConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var storagePath = configuration.GetValue<string>("StoragePath") ?? "/app/videos";
        services.AddSingleton<IVideoStorageService>(new VideoStorageService(storagePath));
        
        return services;
    }
}