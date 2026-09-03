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

    // Devolve false se o ClienteId não existir. No sucesso, preenche o Id gerado pelo banco.
    public async Task<bool> CriarContaAsync(ContaBancaria conta)
    {
        // Regra de negócio: só existe conta com dono.
        var cliente = await _clienteRepository.ObterPorIdAsync(conta.ClienteId);
        if (cliente is null) return false;

        var id = await _contaRepository.CriarContaAsync(conta);
        conta.DefinirId(id);
        return true;
    }

    public Task<ContaBancaria?> ObterPorIdAsync(int id) => _contaRepository.ObterPorIdAsync(id);
}
