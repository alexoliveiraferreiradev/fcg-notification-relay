using Azure.Messaging.ServiceBus;
using MassTransit;
using Microsoft.Extensions.Options;
using NotificationRelayWorkerService.Broker.RabbitMq;
using NotificationRelayWorkerService.Consumers;

namespace NotificationRelayWorkerService.Extensions
{
    public static class MassTransitExtension
    {
        public static IServiceCollection AddMassTransitExtension(this IServiceCollection services)
        {
            services.AddOptions<RabbitMqSettings>()
                .BindConfiguration(RabbitMqSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddMassTransit(x =>
            {
                
                x.AddConsumers(typeof(UserCreatedRelayConsumer).Assembly);

                x.UsingRabbitMq((context,cfg) =>
                {
                    var rabbitMqConfig = context.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
                    
                    cfg.Host(rabbitMqConfig.Host, rabbitMqConfig.Port, "/", h =>
                    {
                        h.Username(rabbitMqConfig.Username);
                        h.Password(rabbitMqConfig.Password);
                    });

                    cfg.UseMessageRetry(r =>
                    {
                        r.Handle<ServiceBusException>(e => e.IsTransient);
                        r.Intervals(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(2));
                    });


                    cfg.ReceiveEndpoint(rabbitMqConfig.NotificationUserCreatedQueue, e =>
                    {
                        e.PrefetchCount = 8;
                        e.ConfigureConsumer<UserCreatedRelayConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(rabbitMqConfig.NotificationPaymentApprovedQueue, e =>
                    {
                        e.PrefetchCount = 8;
                        e.ConfigureConsumer<PaymentApprovedRelayConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(rabbitMqConfig.NotificationPurchaseRejectedQueue, e =>
                    {
                        e.PrefetchCount = 8;
                        e.ConfigureConsumer<PurchaseFailedRelayConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(rabbitMqConfig.NotificationOrderDeliveryFailedQueue, e =>
                    {
                        e.PrefetchCount = 8;
                        e.ConfigureConsumer<DeliveryFailedRelayConsumer>(context);
                    });

                });
            });

            return services;
        }
    }
}
