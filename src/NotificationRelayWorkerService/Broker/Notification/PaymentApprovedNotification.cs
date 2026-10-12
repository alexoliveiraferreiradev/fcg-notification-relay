namespace NotificationRelayWorkerService.Broker.Notification
{
    internal record PaymentApprovedNotification(
    Guid EventId,
    Guid OrderId,
    Guid UserId,
    IEnumerable<Guid> GameIds,
    DateTime CreatedAt);
}
