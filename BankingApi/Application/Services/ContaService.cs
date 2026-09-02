using BankingApi.Domain.Exceptions;
using BankingApi.Domain.Interfaces;
using BankingApi.Domain.Models;

namespace BankingApi.Application.Services;

public class ContaService
{
    private readonly IContaRepository _contaRepository;
    private readonly IClienteRepository _clienteRepository;

    public ContaService(IContaRepository contaRepository, IClienteRepository clienteRepository)
    {
        _contaRepository = contaRepository;
        _clienteRepository = clienteRepository;
    }

    /// <summary>Cria a conta e devolve o Id gerado.</summary>
    /// <exception cref="DominioException">Quando o cliente informado não existe.</exception>
    public async Task<int> CriarContaAsync(ContaBancaria conta)
    {
        // Regra de negócio: só existe conta com dono.
        var cliente = await _clienteRepository.ObterPorIdAsync(conta.ClienteId);
        if (cliente is null)
            throw new DominioException($"Cliente {conta.ClienteId} não encontrado.");

        var id = await _contaRepository.CriarContaAsync(conta);
        conta.DefinirId(id);
        return id;
    }

    public Task<ContaBancaria?> ObterPorIdAsync(int id) => _contaRepository.ObterPorIdAsync(id);
}
