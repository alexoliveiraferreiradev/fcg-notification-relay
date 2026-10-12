using System.ComponentModel.DataAnnotations;

namespace NotificationRelayWorkerService.Broker.MessageBus
{
    public class ServiceBusSettings
    {
        public const string SectionName = "ServiceBus";
        [Required]
        public string ConnectionString { get; init; } = string.Empty;
    }
}
