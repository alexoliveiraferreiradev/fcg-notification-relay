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
        [Required(ErrorMessage = "PaymentOrderQueue é obrigatório")]
        public string PaymentOrderQueue { get; set; } = string.Empty;
        [Required(ErrorMessage = "PaymentDeliveryFailedQueue é obrigatório")]
        public string PaymentDeliveryFailedQueue { get; set; } = string.Empty;
    }
}
