namespace NotificationRelayWorkerService.Abstractions
{
    public enum NotificationQueue
    {
        UserCreated = 1,
        PaymentApproved = 2,
        PurchaseRejected = 3,
        OrderDeliveryFailed = 4
    }

    public static class NotificationQueueSettings
    {
        public static string ToQueueName(this NotificationQueue queue)
        {
            return queue switch
            {
                NotificationQueue.UserCreated => "user-created-queue",
                NotificationQueue.PaymentApproved => "payment-approved-queue",
                NotificationQueue.PurchaseRejected => "purchase-rejected-queue",
                NotificationQueue.OrderDeliveryFailed => "order-delivery-failed-queue",
                _ => throw new ArgumentOutOfRangeException(nameof(queue), queue, null)
            };
        }
    }
}
