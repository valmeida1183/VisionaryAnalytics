using Application.Abstractions.MessageBus;
using Application.Abstractions.QrCodeAnalyzer;
using Application.Abstractions.Repositories;
using Application.Abstractions.Storage;
using Application.Abstractions.VideoAnalyser;
using Domain.ValueObjects;
using FluentValidation;
using Infraestructure.Database;
using Infraestructure.Database.Repositories;
using Infraestructure.MessageBus;
using Infraestructure.QrCodeAnalyzer;
using Infraestructure.Storage;
using Infraestructure.VideoAnalyser;
using MassTransit;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Reflection;

namespace CrossCutting.DI;
public static class DependencyInjection
{
    public static IServiceCollection AddWebApiConfiguration(this IServiceCollection services)
    {
        //services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.Configure<FormOptions>(options =>
        {
            // Set file size limit to 200 MB on Asp.net middleware
            options.MultipartBodyLengthLimit = 200_000_000; 
        });

        return services;
    }

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
        services.Configure<FileStorageSettings>(
            configuration.GetSection("FileStorageSettings"));

        services.AddSingleton<IVideoStorageService, VideoStorageService>();
        services.AddScoped<IVideoFrameAnalyzerService, FFMpegVideoAnalyzerService>();
        services.AddScoped<IQrCodeAnalyzerService, ZxingQrCodeAnalyzerService>();
        services.AddScoped<IMessageEventBusService, MessageEventBusService>();

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