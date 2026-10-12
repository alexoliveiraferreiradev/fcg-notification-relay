namespace NotificationRelayWorkerService.Broker.Notification
{
    internal record PurchaseRejectNotification(
        Guid EventId,
        Guid OrderId,
        Guid UserId,
        string Reason);
}
