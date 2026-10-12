using Fcg.Core.SharedContracts.MessageContracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationRelayWorkerService.Abstractions;
using NotificationRelayWorkerService.Broker.Notification;
using NotificationRelayWorkerService.Consumers;

namespace NotificationRelayWorkerServiceTests.Consumer
{
    public class PaymentApprovedRelayConsumerTest
    {
        private readonly Mock<IBusService> _busServiceMock;
        private readonly Mock<ILogger<PaymentApprovedRelayConsumer>> _loggerMock;

        private readonly PaymentApprovedRelayConsumer _handler;

        public PaymentApprovedRelayConsumerTest()
        {
            _busServiceMock = new Mock<IBusService>();
            _loggerMock = new Mock<ILogger<PaymentApprovedRelayConsumer>>();

            _handler = new PaymentApprovedRelayConsumer(_busServiceMock.Object, _loggerMock.Object);
        }

        private static Mock<ConsumeContext<IPaymentApprovedIntegrationEvent>> CreateContext(Guid eventId, Guid orderId, Guid userId, IEnumerable<Guid> gameIds, DateTime createdAt)
        {
            var paymentApprovedEvent = new Mock<IPaymentApprovedIntegrationEvent>();
            paymentApprovedEvent.SetupGet(e => e.EventId).Returns(eventId);
            paymentApprovedEvent.SetupGet(e => e.OrderId).Returns(orderId);
            paymentApprovedEvent.SetupGet(e => e.UserId).Returns(userId);
            paymentApprovedEvent.SetupGet(e => e.GameIds).Returns(gameIds);
            paymentApprovedEvent.SetupGet(e => e.CreatedAt).Returns(createdAt);

            var consumeContextMock = new Mock<ConsumeContext<IPaymentApprovedIntegrationEvent>>();
            consumeContextMock.SetupGet(c => c.Message).Returns(paymentApprovedEvent.Object);
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
            var gameIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var createdAt = DateTime.UtcNow;
            var context = CreateContext(eventId, orderId, userId, gameIds, createdAt);
            // Act
            await _handler.Consume(context.Object);
            // Assert
            _busServiceMock.Verify(b => b.SendMessageAsync(
                NotificationQueue.PaymentApproved,
                It.Is<object>(p => p is PaymentApprovedNotification
                    && ((PaymentApprovedNotification)p).EventId == eventId
                    && ((PaymentApprovedNotification)p).OrderId == orderId
                    && ((PaymentApprovedNotification)p).UserId == userId
                    && ((PaymentApprovedNotification)p).GameIds.SequenceEqual(gameIds)
                    && ((PaymentApprovedNotification)p).CreatedAt == createdAt),
                eventId,
                CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Consume_ShouldRethrowException_WhenBusServiceFails()
        {
            // Arrange
            var context = CreateContext(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new List<Guid> { Guid.NewGuid() }, DateTime.UtcNow);
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
            var context = CreateContext(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), new List<Guid> { Guid.NewGuid() }, DateTime.UtcNow);
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
