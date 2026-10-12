using Fcg.Core.SharedContracts.MessageContracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationRelayWorkerService.Abstractions;
using NotificationRelayWorkerService.Broker.Notification;
using NotificationRelayWorkerService.Consumers;

namespace NotificationRelayWorkerServiceTests.Consumer
{
    public class PurchaseFailedRelayConsumerTest
    {
        private readonly Mock<IBusService> _busServiceMock;
        private readonly Mock<ILogger<PurchaseFailedRelayConsumer>> _loggerMock;

        private readonly PurchaseFailedRelayConsumer _handler;

        public PurchaseFailedRelayConsumerTest()
        {
            _busServiceMock = new Mock<IBusService>();
            _loggerMock = new Mock<ILogger<PurchaseFailedRelayConsumer>>();

            _handler = new PurchaseFailedRelayConsumer(_busServiceMock.Object, _loggerMock.Object);
        }

        private static Mock<ConsumeContext<IPurchaseRejectedIntegrationEvent>> CreateContext(Guid eventId, Guid orderId, Guid userId, string reason)
        {
            var purchaseFailedEvent = new Mock<IPurchaseRejectedIntegrationEvent>();
            purchaseFailedEvent.SetupGet(e => e.EventId).Returns(eventId);
            purchaseFailedEvent.SetupGet(e => e.OrderId).Returns(orderId);
            purchaseFailedEvent.SetupGet(e => e.UserId).Returns(userId);
            purchaseFailedEvent.SetupGet(e => e.Reason).Returns(reason);

            var consumeContextMock = new Mock<ConsumeContext<IPurchaseRejectedIntegrationEvent>>();
            consumeContextMock.SetupGet(c => c.Message).Returns(purchaseFailedEvent.Object);
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
            var context = CreateContext(eventId, orderId, userId, "Saldo insuficiente");
            // Act
            await _handler.Consume(context.Object);
            // Assert
            _busServiceMock.Verify(b => b.SendMessageAsync(
                NotificationQueue.PurchaseRejected,
                It.Is<object>(p => p is PurchaseRejectNotification
                    && ((PurchaseRejectNotification)p).EventId == eventId
                    && ((PurchaseRejectNotification)p).OrderId == orderId
                    && ((PurchaseRejectNotification)p).UserId == userId
                    && ((PurchaseRejectNotification)p).Reason == "Saldo insuficiente"),
                eventId,
                CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Consume_ShouldRethrowException_WhenBusServiceFails()
        {
            // Arrange
            var context = CreateContext(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Saldo insuficiente");
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
            var context = CreateContext(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), "Saldo insuficiente");
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
