namespace BankingApi.Domain.Interfaces;

// Interface é um contrato: diz O QUE vai ser feito, mas não COMO.
// Quem implementa (NotificadorEmail) fica na camada de Infrastructure.
public interface INotificador
{
    void Notificar(string mensagem);
}
