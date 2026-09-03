using BankingApi.Domain.Models;

namespace BankingApi.Domain.Interfaces;

public interface IContaRepository
{
    // Devolve o Id gerado pelo banco.
    Task<int> CriarContaAsync(ContaBancaria conta);

    Task<ContaBancaria?> ObterPorIdAsync(int id);
    Task<bool> ExisteAsync(int id);

    // false = conta não existe, ou saldo insuficiente, ou valor inválido.
    Task<bool> SacarAsync(int contaId, decimal valor);

    // false = conta não existe, ou valor inválido.
    Task<bool> DepositarAsync(int contaId, decimal valor);
}
