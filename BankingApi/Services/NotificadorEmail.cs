using BankingApi.Interfaces;


namespace BankingApi.Services;

public class NotificadorEmail : INotificador
{

    public void Notificar(string mensagem)
    {
        // Lógica para enviar notificação por e-mail
        Console.WriteLine($"Notificação por e-mail: {mensagem}");
    }
}




