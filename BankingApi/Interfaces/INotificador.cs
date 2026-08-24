namespace BankingApi.Interfaces
{
    public interface INotificador

    //Interface seria um contrato que diz o que ele vai fazer , mas não diz como ele vai fazer
    //Ou seja uma assinatura do contrato 
    {
        void Notificar(string mensagem);
    }
}
