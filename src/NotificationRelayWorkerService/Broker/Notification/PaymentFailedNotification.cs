using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationRelayWorkerService.Broker.Notification
{
    internal record PaymentFailedNotification(
        Guid EventId,
        Guid OrderId,
        Guid UserId,
        string Reason);
}
