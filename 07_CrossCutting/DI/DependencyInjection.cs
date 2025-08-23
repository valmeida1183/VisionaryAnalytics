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
using Application.Abstractions.VideoAnalyser;
using Infraestructure.VideoAnalyser;

namespace CrossCutting.DI;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationConfiguration(this IServiceCollection services)
    {
        var appAssembly = Assembly.Load("03_Application");

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(appAssembly)
        );
        services.AddValidatorsFromAssembly(appAssembly, includeInternalTypes: true);
        
        return services;
    }

    public static IServiceCollection AddInfraestructureConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var storagePath = configuration.GetValue<string>("StoragePath") ?? "/app/videos";
        var ffmpegPath = configuration.GetValue<string>("FFMpegPath") ?? "/usr/bin/ffmpeg";

        services.AddSingleton<IVideoStorageService>(new VideoStorageService(storagePath));
        services.AddScoped<IVideoFrameAnalyserService>(provider => new VideoFrameAnalyserService(ffmpegPath));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IVideoProcessRepository, VideoProcessRepository>();
        services.AddScoped<IVideoQrCodeRepository, VideoQrCodeRepository>();

        return services;
    }

    public static IServiceCollection AddDataBaseConfiguration(this IServiceCollection services, IConfiguration configuration)
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

    public static IServiceCollection AddMassTransitProducerConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var host = configuration.GetSection("RabbitMq:Host").Value;
        var user = configuration.GetSection("RabbitMq:User").Value ?? "guest";
        var password = configuration.GetSection("RabbitMq:Password").Value ?? "guest";

        if (string.IsNullOrEmpty(host) ||
            string.IsNullOrEmpty(user) ||
            string.IsNullOrEmpty(password))
            throw new Exception("Missing environment variables to configure MassTransit");

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

    public static IServiceCollection AddMassTransitConsumerConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var host = configuration.GetSection("RabbitMq:Host").Value;
        var user = configuration.GetSection("RabbitMq:User").Value ?? "guest";
        var password = configuration.GetSection("RabbitMq:Password").Value ?? "guest";

        if (string.IsNullOrEmpty(host) ||
            string.IsNullOrEmpty(user) ||
            string.IsNullOrEmpty(password))
            throw new Exception("Missing environment variables to configure MassTransit");

        services.AddMassTransit(x =>
        {
            var assembly = Assembly.Load("02_QueueConsumer");
            x.AddConsumers(assembly);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(host, "/", h =>
                {
                    h.Username(user);
                    h.Password(password);
                });

                cfg.ConfigureEndpoints(context);

            });
        });    

        return services;
    }
}