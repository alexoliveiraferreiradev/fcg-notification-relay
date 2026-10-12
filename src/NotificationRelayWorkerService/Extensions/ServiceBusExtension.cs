using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using NotificationRelayWorkerService.Abstractions;
using NotificationRelayWorkerService.Broker.MessageBus;

namespace NotificationRelayWorkerService.Extensions
{
    public static class ServiceBusExtension
    {
        public static IServiceCollection AddServiceBus(this IServiceCollection services)
        {
            services.AddOptions<ServiceBusSettings>()
                .BindConfiguration(ServiceBusSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

           
            services.AddSingleton<IBusService, AzureServiceBusService>();

            services.AddSingleton(sp => new ServiceBusClient(sp.GetRequiredService<IOptions<ServiceBusSettings>>().Value.ConnectionString));

            return services;
        }
    }
}
