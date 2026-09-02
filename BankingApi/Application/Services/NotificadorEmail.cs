using BankingApi.Domain.Interfaces;

namespace BankingApi.Services;

// A camada de Service isola regras de infraestrutura e serviços externos.
// Esta classe implementa o contrato 'INotificador', garantindo baixo acoplamento.
public class NotificadorEmail : INotificador
{
    // Implementação real do método exigido pela interface INotificador.
    // Pode ser injetada e reutilizada em qualquer ponto do sistema via DI.
    public void Notificar(string mensagem)
    {
        // Simulação do envio real de e-mail (por enquanto, log no console)
        Console.WriteLine($"Notificação por e-mail: {mensagem}");
    }
}