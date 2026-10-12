using System.ComponentModel.DataAnnotations;

namespace NotificationRelayWorkerService.Broker.RabbitMq
{
    public class RabbitMqSettings 
    {
        public const string SectionName = "RabbitMqSettings";

        [Required(ErrorMessage = "Host do RabbitMQ é obrigatório.")]
        public string Host { get; set; } = string.Empty;

        [Range(1, 65535, ErrorMessage = "Porta do RabbitMQ deve estar entre 1 e 65535.")]
        public ushort Port { get; set; } = 5672;

        [Required(ErrorMessage = "Nome de usuário é obrigatório.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória.")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "NotificationUserCreatedQueue é obrigatório")]
        public string NotificationUserCreatedQueue { get; set; } = string.Empty;
        [Required(ErrorMessage = "NotificationOrderDeliveryFailedQueue é obrigatório")]
        public string NotificationOrderDeliveryFailedQueue { get; set; } = string.Empty;
        [Required(ErrorMessage = "NotificationPaymentApprovedQueue é obrigatório")]
        public string NotificationPaymentApprovedQueue { get; set; } = string.Empty;
        [Required(ErrorMessage = "NotificationPurchaseRejectedQueue é obrigatório")]
        public string NotificationPurchaseRejectedQueue { get; set; } = string.Empty;
    }
}
