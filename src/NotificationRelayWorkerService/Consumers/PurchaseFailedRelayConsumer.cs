using Fcg.Core.SharedContracts.MessageContracts;
using MassTransit;
using NotificationRelayWorkerService.Abstractions;
using NotificationRelayWorkerService.Broker.Notification;
using System.Text.Json;

namespace NotificationRelayWorkerService.Consumers
{
    public class PurchaseFailedRelayConsumer : IConsumer<IPurchaseRejectedIntegrationEvent>
    {
        private readonly IBusService _service;
        private readonly ILogger<PurchaseFailedRelayConsumer> _logger;

        public PurchaseFailedRelayConsumer(IBusService service, ILogger<PurchaseFailedRelayConsumer> logger)
        {
            _service = service;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<IPurchaseRejectedIntegrationEvent> context)
        {
            try
            {
                var message = context.Message;
                if(message.EventId == Guid.Empty)
                {
                    _logger.LogError("Mensagem de pagamento falho inválida. EventId não pode ser vazio.");
                    throw new ArgumentException("Mensagem de pagamento falho inválida. EventId não pode ser vazio.");
                }
                _logger.LogInformation("Mensagem de pagamento falho recebida. EventId: {EventId}, OrderId: {OrderId}, UserId: {UserId}", message.EventId, message.OrderId, message.UserId);
                var purchaseRejectNotification = new PurchaseRejectNotification
                (
                    message.EventId,
                    message.OrderId,
                    message.UserId,
                    message.Reason
                );
                _logger.LogInformation("Processando mensagem de pagamento falho. EventId: {EventId}, OrderId: {OrderId}, UserId: {UserId}", context.Message.EventId, context.Message.OrderId, context.Message.UserId);
                await _service.SendMessageAsync(NotificationQueue.PurchaseRejected, purchaseRejectNotification, message.EventId, context.CancellationToken);
            }
            catch
            {
                _logger.LogError("Erro ao processar a mensagem: {Message}", JsonSerializer.Serialize(context.MessageId));
                throw;
            }
        }

    }
}
