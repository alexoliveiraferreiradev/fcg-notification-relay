namespace NotificationRelayWorkerService.Broker.Notification
{
    internal record RepublishUserNotification(
     Guid EventId,
     Guid UserId,
     string Name,
     string Email,
     DateTime CreatedAt);
}

