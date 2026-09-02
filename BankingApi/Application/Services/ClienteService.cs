using BankingApi.Domain.Exceptions;
using BankingApi.Domain.Interfaces;
using BankingApi.Domain.Models;

namespace BankingApi.Application.Services;

public class ClienteService
{
    // Depende da INTERFACE do repositório, não do BancoDbContext nem do Dapper.
    // Trocar SQLite por SQL Server não muda uma linha desta classe.
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    /// <summary>Cadastra o cliente e devolve o Id gerado.</summary>
    /// <exception cref="DominioException">Quando o CPF já existe.</exception>
    public async Task<int> CriarClienteAsync(Cliente cliente)
    {
        // Regra de negócio: CPF é único.
        if (await _clienteRepository.ExisteCpfAsync(cliente.Cpf))
            throw new DominioException("Já existe um cliente cadastrado com esse CPF.");

        var id = await _clienteRepository.AdicionarAsync(cliente);
        cliente.DefinirId(id);
        return id;
    }

    public Task<Cliente?> ObterPorIdAsync(int id) => _clienteRepository.ObterPorIdAsync(id);
}
