using BankingApi.Domain.Interfaces;
using BankingApi.Domain.Models;

namespace BankingApi.Application.Services;

public class TransacaoService
{
    private readonly IContaRepository _contaRepository;
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly INotificador _notificador;

    public TransacaoService(
        IContaRepository contaRepository,
        ITransacaoRepository transacaoRepository,
        INotificador notificador)
    {
        _contaRepository = contaRepository;
        _transacaoRepository = transacaoRepository;
        _notificador = notificador;
    }

    public async Task<bool> DepositarAsync(int contaId, decimal valor)
    {
        // 1. Movimenta o saldo da conta
        if (!await _contaRepository.DepositarAsync(contaId, valor)) return false;

        // 2. Grava o lançamento no extrato
        await _transacaoRepository.AdicionarAsync(new Transacao(valor, "Deposito", contaId));

        _notificador.Notificar($"Depósito de {valor:C} realizado na conta {contaId}.");
        return true;
    }

    public async Task<bool> SacarAsync(int contaId, decimal valor)
    {
        // 1. Movimenta o saldo da conta
        if (!await _contaRepository.SacarAsync(contaId, valor)) return false;

        // 2. Grava o lançamento no extrato
        await _transacaoRepository.AdicionarAsync(new Transacao(valor, "Saque", contaId));

        _notificador.Notificar($"Saque de {valor:C} realizado na conta {contaId}.");
        return true;
    }

    public async Task<bool> TransferirAsync(int contaOrigemId, int contaDestinoId, decimal valor)
    {
        if (contaOrigemId == contaDestinoId) return false;

        // 1. Saque na origem e depósito no destino
        if (!await _contaRepository.SacarAsync(contaOrigemId, valor)) return false;
        if (!await _contaRepository.DepositarAsync(contaDestinoId, valor)) return false;

        // 2. Registra os dois lados no extrato
        await _transacaoRepository.AdicionarAsync(
            new Transacao(valor, "Transferencia Enviada", contaOrigemId));
        await _transacaoRepository.AdicionarAsync(
            new Transacao(valor, "Transferencia Recebida", contaDestinoId));

        _notificador.Notificar(
            $"Transferência de {valor:C} da conta {contaOrigemId} para a conta {contaDestinoId}.");
        return true;
    }

    // Do lançamento mais recente para o mais antigo. Devolve null se a conta não existir.
    public async Task<IReadOnlyList<Transacao>?> ObterExtratoAsync(int contaId)
    {
        if (!await _contaRepository.ExisteAsync(contaId)) return null;

        var lancamentos = (await _transacaoRepository.ObterPorContaIdAsync(contaId)).ToList();

        _notificador.Notificar(
            $"Extrato gerado para a conta {contaId}. Total de lançamentos: {lancamentos.Count}.");

        return lancamentos;
    }
}
