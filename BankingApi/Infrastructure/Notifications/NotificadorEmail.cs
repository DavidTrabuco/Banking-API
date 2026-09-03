using BankingApi.Domain.Interfaces;

namespace BankingApi.Infrastructure.Notifications;

// Envio de e-mail é detalhe de infraestrutura, por isso mora aqui e não em Application.
// O Service depende só da interface INotificador.
public class NotificadorEmail : INotificador
{
    private readonly ILogger<NotificadorEmail> _logger;

    public NotificadorEmail(ILogger<NotificadorEmail> logger)
    {
        _logger = logger;
    }

    public void Notificar(string mensagem)
    {
        // Simulação do envio real (por enquanto, apenas log).
        _logger.LogInformation("Notificação por e-mail: {Mensagem}", mensagem);
    }
}
