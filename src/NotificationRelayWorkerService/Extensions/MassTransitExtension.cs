using Azure.Messaging.ServiceBus;
using MassTransit;
using Microsoft.Extensions.Options;
using NotificationRelayWorkerService.Broker.RabbitMq;
using NotificationRelayWorkerService.Consumers;

namespace NotificationRelayWorkerService.Extensions
{
    public static class MassTransitExtension
    {
        public static void AddMassTransit(this HostApplicationBuilder builder)
        {
            builder.Services.AddMassTransit(x =>
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


                    cfg.ReceiveEndpoint("notification-user-created", e =>
                    {
                        e.PrefetchCount = 8;
                        e.ConfigureConsumer<UserCreatedRelayConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("notification-payment-approved", e =>
                    {
                        e.PrefetchCount = 8;
                        e.ConfigureConsumer<PaymentApprovedRelayConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("notification-payment-failed", e =>
                    {
                        e.PrefetchCount = 8;
                        e.ConfigureConsumer<PaymentFailedRelayConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("notification-delivery-failed", e =>
                    {
                        e.PrefetchCount = 8;
                        e.ConfigureConsumer<DeliveryFailedRelayConsumer>(context);
                    });

                });
            });
        }
    }
}
