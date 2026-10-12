using Fcg.Core.SharedContracts.MessageContracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationRelayWorkerService.Abstractions;
using NotificationRelayWorkerService.Broker.Notification;
using NotificationRelayWorkerService.Consumers;

namespace NotificationRelayWorkerServiceTests.Consumer
{
    public class UserCreatedRelayConsumerTest
    {
        private readonly Mock<IBusService> _busServiceMock;
        private readonly Mock<ILogger<UserCreatedRelayConsumer>> _loggerMock;

        private readonly UserCreatedRelayConsumer _handler;

        public UserCreatedRelayConsumerTest()
        {
            _busServiceMock = new Mock<IBusService>();
            _loggerMock = new Mock<ILogger<UserCreatedRelayConsumer>>();

            _handler = new UserCreatedRelayConsumer(_busServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Consume_ShouldSendMessageToBusService_WhenUserCreatedEventIsReceived()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var nameUser = "Teste Usuario";
            var emailUser = "teste@email.com";
            var userCreatedEvent = new Mock<IUserCreatedIntegrationEvent>();
            userCreatedEvent.SetupGet(e => e.EventId).Returns(Guid.NewGuid());
            userCreatedEvent.SetupGet(e => e.UserId).Returns(userId);
            userCreatedEvent.SetupGet(e => e.Name).Returns(nameUser);
            userCreatedEvent.SetupGet(e => e.Email).Returns(emailUser);
            var consumeContextMock = new Mock<MassTransit.ConsumeContext<IUserCreatedIntegrationEvent>>();
            consumeContextMock.SetupGet(c => c.Message).Returns(userCreatedEvent.Object);
            consumeContextMock.SetupGet(c => c.CancellationToken).Returns(CancellationToken.None);
            // Act
            await _handler.Consume(consumeContextMock.Object);
            // Assert
            _busServiceMock.Verify(b => b.SendMessageAsync(
                NotificationQueue.UserCreated,
                 It.Is<object>(p => p is UserCreatedNotification
                    && ((UserCreatedNotification)p).UserId == userId
                    && ((UserCreatedNotification)p).Name == nameUser
                    && ((UserCreatedNotification)p).Email == emailUser),
                userCreatedEvent.Object.EventId,
                CancellationToken.None), Times.Once);
        }


        [Fact]
        public async Task Consume_ShouldPropagateException_WhenBusServiceFails()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var nameUser = "Teste Usuario";
            var emailUser = "teste@email.com";
            var userCreatedEvent = new Mock<IUserCreatedIntegrationEvent>();
            userCreatedEvent.SetupGet(e => e.EventId).Returns(Guid.NewGuid());
            userCreatedEvent.SetupGet(e => e.UserId).Returns(userId);
            userCreatedEvent.SetupGet(e => e.Name).Returns(nameUser);
            userCreatedEvent.SetupGet(e => e.Email).Returns(emailUser);
            var consumeContextMock = new Mock<MassTransit.ConsumeContext<IUserCreatedIntegrationEvent>>();
            consumeContextMock.SetupGet(c => c.Message).Returns(userCreatedEvent.Object);
            consumeContextMock.SetupGet(c => c.CancellationToken).Returns(CancellationToken.None);
            _busServiceMock
                .Setup(b => b.SendMessageAsync(It.IsAny<NotificationQueue>(), 
                It.Is<object>(p => p is UserCreatedNotification
                    && ((UserCreatedNotification)p).UserId == userId
                    && ((UserCreatedNotification)p).Name == nameUser
                    && ((UserCreatedNotification)p).Email == emailUser),
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("falha no envio"));

            //Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Consume(consumeContextMock.Object));
        }

        [Fact]
        public async Task Consume_ShouldThrowArgumentException_WhenEventIdIsInvalid()
        {
            //Arrange
            var userCreatedEvent = new Mock<IUserCreatedIntegrationEvent>();
            userCreatedEvent.SetupGet(e => e.EventId).Returns(Guid.Empty);
            var consumeContextMock = new Mock<MassTransit.ConsumeContext<IUserCreatedIntegrationEvent>>();
            consumeContextMock.SetupGet(c => c.Message).Returns(userCreatedEvent.Object);
            consumeContextMock.SetupGet(c => c.CancellationToken).Returns(CancellationToken.None);            
            // Act
            Func<Task> act = () => _handler.Consume(consumeContextMock.Object);
            // Assert
            await Assert.ThrowsAsync<ArgumentException>(act);
            _busServiceMock.Verify(b => b.SendMessageAsync(
                NotificationQueue.UserCreated,
                It.IsAny<object>(),
                userCreatedEvent.Object.EventId,
                CancellationToken.None), Times.Never);
        }
    }
}
