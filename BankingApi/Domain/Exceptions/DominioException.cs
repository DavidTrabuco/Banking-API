namespace BankingApi.Domain.Exceptions;

/// <summary>
/// Erro de regra de negócio (saldo insuficiente, conta inexistente, valor inválido...).
/// É tratada pelo ExceptionMiddleware e vira um HTTP 400, em vez de um 500.
/// </summary>
public class DominioException : Exception
{
    public DominioException(string mensagem) : base(mensagem) { }
}
