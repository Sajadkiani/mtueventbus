using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MtuEventBus.Consumers;
using MtuEventBus.Options;

namespace MtuEventBus.Extensions;

public static class MtuEventBusExtension
{
    public static IServiceCollection AddMtuBus(this IServiceCollection services,
        IConfiguration configuration, string sectionName = "RabbitMq", params Assembly[] consumerAssemblies)
    {
        ConfigureMtuOptions(services, sectionName, configuration);
        AddMtuPublisher(services);
        AddMtuConsumers(services, consumerAssemblies);

        return services;
    }

    private static void ConfigureMtuOptions(IServiceCollection services, string sectionName, IConfiguration configuration)
    {
        var mtuRabbitmq = configuration.GetSection(sectionName);
        if (mtuRabbitmq is null)
        {
            throw new Exception("rabbitmq section not found");
        }

        services.Configure<MtuRabbitMqOptions>(mtuRabbitmq);
    }

    public static IServiceCollection AddMtuConsumers(IServiceCollection services, params Assembly[] assemblies)
    {
        if (assemblies == null || assemblies.Length == 0)
        {
            Console.WriteLine(@"add mtu consumers assemblies is null or empty,
             using current domain assemblies");

            assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }

        var consumerTypes = assemblies
           .SelectMany(x => x.GetTypes())
           .Where(x =>
               x is
               {
                   IsAbstract: false,
                   IsInterface: false
               } &&
               typeof(MtuConsumer)
                   .IsAssignableFrom(x))
           .ToList();

        foreach (var consumerType in consumerTypes)
        {
            services.AddScoped(consumerType);

            using var provider =
                services.BuildServiceProvider();

            using var scope =
                provider.CreateScope();

            var consumer =
                (MtuConsumer)scope.ServiceProvider
                    .GetRequiredService(consumerType);

            var registration = new MtuConsumerRegistration
            {
                ConsumerType = consumerType,
                QueueName = consumer.QueueName,
                RoutingKey = consumer.RoutingKey
            };

            services.AddSingleton(registration);
        }

        // Console.WriteLine("Registered {Count} MTU consumers", consumerTypes.Count);

        services.AddHostedService<MtuBusHostedService>();
        return services;
    }

    private static void AddMtuPublisher(IServiceCollection services)
    {
        services.AddSingleton<IMtuBusConnectionManager, MtuBusConnectionManager>();
        services.AddSingleton<IIntegrationEventDispatcher, IntegrationEventDispatcher>();
    }
}