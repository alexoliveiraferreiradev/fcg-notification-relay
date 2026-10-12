namespace NotificationRelayWorkerService.Abstractions
{
    public enum NotificationQueue
    {
        UserCreated = 1,
        PaymentApproved = 2,
        PurchaseRejected = 3,
        OrderDeliveryFailed = 4
    }

    public static class NotificationQueueExtensions
    {
        public static string ToQueueName(this NotificationQueue queue)
        {
            return queue switch
            {
                NotificationQueue.UserCreated => "notification-user-created-queue",
                NotificationQueue.PaymentApproved => "notification-payment-approved-queue",
                NotificationQueue.PurchaseRejected => "notification-purchase-rejected-queue",
                NotificationQueue.OrderDeliveryFailed => "notification-order-delivery-failed-queue",
                _ => throw new ArgumentOutOfRangeException(nameof(queue), queue, null)
            };
        }
    }
}
