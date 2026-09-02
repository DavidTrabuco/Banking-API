using BankingApi.Domain.Interfaces;

namespace BankingApi.Infrastructure.Notifications;

/// <summary>
/// Implementação do contrato INotificador. Envio de e-mail é um detalhe de
/// infraestrutura, por isso mora aqui e não na camada Application — assim o
/// Service depende só da interface (baixo acoplamento).
/// </summary>
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
