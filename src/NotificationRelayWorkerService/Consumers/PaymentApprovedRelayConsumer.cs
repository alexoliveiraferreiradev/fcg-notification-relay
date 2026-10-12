using Fcg.Core.SharedContracts.MessageContracts;
using MassTransit;
using NotificationRelayWorkerService.Abstractions;
using NotificationRelayWorkerService.Broker.Notification;
using System.Text.Json;

namespace NotificationRelayWorkerService.Consumers
{
    public class PaymentApprovedRelayConsumer : IConsumer<IPaymentApprovedIntegrationEvent>
    {
        private readonly IBusService _service;
        private readonly ILogger<PaymentApprovedRelayConsumer> _logger;

        public PaymentApprovedRelayConsumer(IBusService service, ILogger<PaymentApprovedRelayConsumer> logger)
        {
            _service = service;
            _logger = logger;
        }


        public async Task Consume(ConsumeContext<IPaymentApprovedIntegrationEvent> context)
        {
            try
            {
                var message = context.Message;
                if(message.EventId == Guid.Empty)
                {
                    _logger.LogError("Mensagem de pagamento aprovado inválida. EventId: {EventId}, OrderId: {OrderId}, UserId: {UserId}", context.Message.EventId, context.Message.OrderId, context.Message.UserId);
                    throw new ArgumentException("Mensagem de pagamento aprovado inválida. EventId não pode ser vazio.");
                }
                _logger.LogInformation("Mensagem de pagamento aprovado recebida. EventId: {EventId}, OrderId: {OrderId}, UserId: {UserId}", context.Message.EventId, context.Message.OrderId, context.Message.UserId);
                var paymentApprovedNotification = new PaymentApprovedNotification
                (
                    message.EventId,
                    message.OrderId,
                    message.UserId,
                    message.GameIds,
                    message.CreatedAt
                );
                _logger.LogInformation("Processando mensagem de pagamento aprovado. EventId: {EventId}, OrderId: {OrderId}, UserId: {UserId}", context.Message.EventId, context.Message.OrderId, context.Message.UserId);
                await _service.SendMessageAsync(NotificationQueue.PaymentApproved,
                    paymentApprovedNotification, message.EventId, context.CancellationToken);
            }catch
            {
                _logger.LogError("Erro ao processar a mensagem: {Message}", JsonSerializer.Serialize(context.MessageId));
                throw;
            }
            
        }
    }
}
