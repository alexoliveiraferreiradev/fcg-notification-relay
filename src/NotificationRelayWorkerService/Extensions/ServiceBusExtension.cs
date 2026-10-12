using Azure.Messaging.ServiceBus;
using NotificationRelayWorkerService.Abstractions;
using NotificationRelayWorkerService.Broker.MessageBus;

namespace NotificationRelayWorkerService.Extensions
{
    public static class ServiceBusExtension
    {
        public static void AddServiceBus(this HostApplicationBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddSingleton(new ServiceBusClient(configuration["AzureServiceBus"]));
            builder.Services.AddSingleton<IBusService, AzureServiceBusService>();
        }
    }
}
