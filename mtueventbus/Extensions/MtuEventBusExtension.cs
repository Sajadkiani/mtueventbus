using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MtuEventBus.Consumers;
using MtuEventBus.Options;

namespace MtuEventBus.Extensions;

public static class MtuEventBusExtension
{
    public static IServiceCollection AddMtuBus(this IServiceCollection services, IConfiguration configuration, string sectionName = "RabbitMq")
    {
        ConfigureMtuOptions(services, sectionName, configuration);
        AddMtuPublisher(services);
        AddMtuConsumers(services);

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

    private static IServiceCollection AddMtuConsumers(IServiceCollection services)
    {
        services.AddHostedService<MtuBusHostedService>();
        return services;
    }

    private static void AddMtuPublisher(IServiceCollection services)
    {
        services.AddSingleton<IMtuBusConnectionManager, MtuBusConnectionManager>();
        services.AddSingleton<IIntegrationEventDispatcher, IntegrationEventDispatcher>();
    }
}