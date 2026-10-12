namespace NotificationRelayWorkerService.Broker.Notification
{
    internal record UserCreatedNotification(
     Guid EventId,
     Guid UserId,
     string Name,
     string Email,
     DateTime CreatedAt);
}
