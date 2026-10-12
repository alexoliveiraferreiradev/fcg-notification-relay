using Azure.Messaging.ServiceBus;
using NotificationRelayWorkerService.Abstractions;

namespace NotificationRelayWorkerService.Broker.MessageBus
{
    public class AzureServiceBusService : IBusService
    {
        private readonly ServiceBusClient _client;

        public AzureServiceBusService(ServiceBusClient client)
        {
            _client = client;
        }
               

        public async Task SendMessageAsync(NotificationQueue queue, object payload, Guid messageId, CancellationToken ct)
        {
            await using var sender = _client.CreateSender(queue.ToQueueName());
            var serviceBusMessage = new ServiceBusMessage(BinaryData.FromObjectAsJson(payload))
            {
                MessageId = messageId.ToString(),
                ContentType = "application/json"
            };
            await sender.SendMessageAsync(serviceBusMessage,ct);
        }
    }
}
