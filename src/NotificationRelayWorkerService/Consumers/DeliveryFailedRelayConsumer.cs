using Fcg.Core.SharedContracts.MessageContracts;
using MassTransit;
using NotificationRelayWorkerService.Abstractions;
using NotificationRelayWorkerService.Broker.Notification;
using System.Text.Json;

namespace NotificationRelayWorkerService.Consumers
{
    public class DeliveryFailedRelayConsumer : IConsumer<IDeliveryFailedIntegrationEvent>
    {
        private readonly IBusService _service;
        private readonly ILogger<DeliveryFailedRelayConsumer> _logger;

        public DeliveryFailedRelayConsumer(IBusService service, ILogger<DeliveryFailedRelayConsumer> logger)
        {
            _service = service;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<IDeliveryFailedIntegrationEvent> context)
        {
            try
            {

                var message = context.Message;
                if(message.EventId == Guid.Empty || message.OrderId == Guid.Empty || message.UserId == Guid.Empty)
                {
                    _logger.LogWarning("Mensagem de entrega falhou recebida com dados inválidos: EventId: {EventId}, OrderId: {OrderId}, UserId: {UserId}", message.EventId, message.OrderId, message.UserId);
                    throw new ArgumentException("Mensagem de entrega falhou recebida com dados inválidos.");
                    
                }
                _logger.LogInformation("Mensagem de entrega falhou recebida:EventId: {EventId}, OrderId: {OrderId}, UserId: {UserId}", message.EventId, message.OrderId, message.UserId);
                var deliveryFailedNotification = new DeliveyFailedNotification
                (
                    message.EventId,
                    message.OrderId,
                    message.UserId,
                    message.Reason
                );
                _logger.LogInformation("Processando mensagem de entrega falhou. EventId: {EventId}, OrderId: {OrderId}, UserId: {UserId}", deliveryFailedNotification.EventId, deliveryFailedNotification.OrderId, deliveryFailedNotification.UserId);
                await _service.SendMessageAsync(NotificationQueue.PurchaseRejected, deliveryFailedNotification, message.EventId, context.CancellationToken);
            }
            catch
            {
                _logger.LogError("Erro ao processar a mensagem: {Message}", JsonSerializer.Serialize(context.MessageId));
                throw;
            }
        }
    }
}
