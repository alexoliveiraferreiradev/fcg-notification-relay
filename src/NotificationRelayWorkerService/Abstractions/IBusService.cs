namespace NotificationRelayWorkerService.Abstractions
{
    public interface IBusService
    {
        Task SendMessageAsync(NotificationQueue queue, object payload, Guid messageId, CancellationToken ct);  
    }
}
