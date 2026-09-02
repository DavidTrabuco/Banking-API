using BankingApi.Domain.Models;

namespace BankingApi.Domain.Interfaces;

public interface IClienteRepository
{
    Task<bool> ExisteCpfAsync(string cpf);

    /// <summary>Insere o cliente e devolve o Id gerado pelo banco.</summary>
    Task<int> AdicionarAsync(Cliente cliente);

    Task<Cliente?> ObterPorIdAsync(int id);
}
