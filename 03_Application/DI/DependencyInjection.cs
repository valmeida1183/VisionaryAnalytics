//using FluentValidation;
//using Microsoft.Extensions.DependencyInjection;

//namespace Application.DI;

//public static class DependencyInjection
//{
//    public static IServiceCollection AddApplication(this IServiceCollection services)
//    {
//        // Register application services, commands, queries, etc.
//        services.AddMediatR(cfg => 
//            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly)
//        );
//        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

//        // Register other application-specific services here
//        // e.g., services.AddScoped<IYourService, YourService>();
//        return services;
//    }
//}
