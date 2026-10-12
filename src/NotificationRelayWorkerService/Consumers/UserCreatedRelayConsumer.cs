using Fcg.Core.SharedContracts.MessageContracts;
using MassTransit;
using NotificationRelayWorkerService.Abstractions;
using NotificationRelayWorkerService.Broker.Notification;
using System.Text.Json;

namespace NotificationRelayWorkerService.Consumers
{
    public class UserCreatedRelayConsumer : IConsumer<IUserCreatedIntegrationEvent>
    {
        private readonly IBusService _service;
        private readonly ILogger<UserCreatedRelayConsumer> _logger;

        public UserCreatedRelayConsumer(IBusService service, ILogger<UserCreatedRelayConsumer> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IUserCreatedIntegrationEvent> context)
        {
            try
            {
                var message = context.Message;
                if (message.EventId == Guid.Empty)
                {
                    _logger.LogWarning("ID do evento inválido: {EventId}", message.EventId);
                    throw new ArgumentException("O ID do evento é inválido.", nameof(message.EventId));
                }
                _logger.LogInformation("Mensagem recebida: {Message}", JsonSerializer.Serialize(message));
                var userNotification = new UserCreatedNotification
                (
                     message.EventId,
                     message.UserId,
                    message.Name,
                    message.Email,
                     message.CreatedAt
                );
                await _service.SendMessageAsync(NotificationQueue.UserCreated, userNotification, message.EventId, context.CancellationToken);
                _logger.LogInformation("Mensagem enviada para a fila 'user-created-queue': {Message}", message.EventId);
            }
            catch
            {
                _logger.LogError("Erro ao processar a mensagem: {Message}", JsonSerializer.Serialize(context.MessageId));
                throw;
            }
        }
    }
}
