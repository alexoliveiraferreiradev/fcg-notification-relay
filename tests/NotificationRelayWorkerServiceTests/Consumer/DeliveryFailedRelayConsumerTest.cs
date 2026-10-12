using Fcg.Core.SharedContracts.MessageContracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationRelayWorkerService.Abstractions;
using NotificationRelayWorkerService.Broker.Notification;
using NotificationRelayWorkerService.Consumers;

namespace NotificationRelayWorkerServiceTests.Consumer
{
    public class DeliveryFailedRelayConsumerTest
    {
        private readonly Mock<IBusService> _busServiceMock;
        private readonly Mock<ILogger<DeliveryFailedRelayConsumer>> _loggerMock;

        private readonly DeliveryFailedRelayConsumer _handler;

        public DeliveryFailedRelayConsumerTest()
        {
            _busServiceMock = new Mock<IBusService>();
            _loggerMock = new Mock<ILogger<DeliveryFailedRelayConsumer>>();

            _handler = new DeliveryFailedRelayConsumer(_busServiceMock.Object, _loggerMock.Object);
        }

        private static Mock<ConsumeContext<IDeliveryFailedIntegrationEvent>> CreateContext(Guid eventId, Guid orderId, Guid userId, string reason)
        {
            var deliveryFailedEvent = new Mock<IDeliveryFailedIntegrationEvent>();
            deliveryFailedEvent.SetupGet(e => e.EventId).Returns(eventId);
            deliveryFailedEvent.SetupGet(e => e.OrderId).Returns(orderId);
            deliveryFailedEvent.SetupGet(e => e.UserId).Returns(userId);
            deliveryFailedEvent.SetupGet(e => e.Reason).Returns(reason);

            var consumeContextMock = new Mock<ConsumeContext<IDeliveryFailedIntegrationEvent>>();
            consumeContextMock.SetupGet(c => c.Message).Returns(deliveryFailedEvent.Object);
            consumeContextMock.SetupGet(c => c.CancellationToken).Returns(CancellationToken.None);
            return consumeContextMock;
        }

        [Fact]
        public async Task Consume_ShouldSendNotificationToBusService_WhenEventIsValid()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var context = CreateContext(eventId, orderId, userId, "Endereço não encontrado");
            // Act
            await _handler.Consume(context.Object);
            // Assert
            _busServiceMock.Verify(b => b.SendMessageAsync(
                NotificationQueue.PurchaseRejected,
                It.Is<object>(p => p is DeliveyFailedNotification
                    && ((DeliveyFailedNotification)p).EventId == eventId
                    && ((DeliveyFailedNotification)p).OrderId == orderId
                    && ((DeliveyFailedNotification)p).UserId == userId
                    && ((DeliveyFailedNotification)p).Reason == "Endereço não encontrado"),
                eventId,
                CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Consume_ShouldRethrowException_WhenBusServiceFails()
        {
            // Arrange
            var context = CreateContext(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Endereço não encontrado");
            _busServiceMock
                .Setup(b => b.SendMessageAsync(It.IsAny<NotificationQueue>(), It.IsAny<object>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Falha no barramento"));
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Consume(context.Object));
        }

        [Fact]
        public async Task Consume_ShouldThrowArgumentException_WhenEventIdIsEmpty()
        {
            // Arrange
            var context = CreateContext(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), "Endereço não encontrado");
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _handler.Consume(context.Object));
            _busServiceMock.Verify(b => b.SendMessageAsync(
                It.IsAny<NotificationQueue>(),
                It.IsAny<object>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
