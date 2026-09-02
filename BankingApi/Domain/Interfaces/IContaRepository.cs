using BankingApi.Domain.Models;

namespace BankingApi.Domain.Interfaces;

public interface IContaRepository
{
    /// <summary>Insere a conta e devolve o Id gerado pelo banco.</summary>
    Task<int> CriarContaAsync(ContaBancaria conta);

    Task<ContaBancaria?> ObterPorIdAsync(int id);
    Task<bool> ExisteAsync(int id);

    Task SacarAsync(int contaId, decimal valor);
    Task DepositarAsync(int contaId, decimal valor);
}
