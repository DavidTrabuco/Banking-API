using BankingApi.Domain.Interfaces;
using BankingApi.Domain.Models;

namespace BankingApi.Application.Services;

public class ClienteService
{
    // Depende da INTERFACE do repositório, não do BancoDbContext nem do Dapper.
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    // Devolve false se o CPF já existir. No sucesso, preenche o Id gerado pelo banco.
    public async Task<bool> CriarClienteAsync(Cliente cliente)
    {
        // Regra de negócio: CPF é único.
        var cpfExiste = await _clienteRepository.ExisteCpfAsync(cliente.Cpf);
        if (cpfExiste) return false;

        var id = await _clienteRepository.AdicionarAsync(cliente);
        cliente.DefinirId(id);
        return true;
    }

    public Task<Cliente?> ObterPorIdAsync(int id) => _clienteRepository.ObterPorIdAsync(id);
}
