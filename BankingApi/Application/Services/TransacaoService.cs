using BankingApi.Domain.Exceptions;
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

    public async Task DepositarAsync(int contaId, decimal valor)
    {
        // 1. Movimenta o saldo da conta
        await _contaRepository.DepositarAsync(contaId, valor);

        // 2. Grava o lançamento no extrato
        await _transacaoRepository.AdicionarAsync(new Transacao(valor, "Deposito", contaId));

        _notificador.Notificar($"Depósito de {valor:C} realizado na conta {contaId}.");
    }

    public async Task SacarAsync(int contaId, decimal valor)
    {
        // 1. Movimenta o saldo da conta
        await _contaRepository.SacarAsync(contaId, valor);

        // 2. Grava o lançamento no extrato
        await _transacaoRepository.AdicionarAsync(new Transacao(valor, "Saque", contaId));

        _notificador.Notificar($"Saque de {valor:C} realizado na conta {contaId}.");
    }

    public async Task TransferirAsync(int contaOrigemId, int contaDestinoId, decimal valor)
    {
        if (contaOrigemId == contaDestinoId)
            throw new DominioException("A conta de origem e a de destino não podem ser a mesma.");

        // 1. Saque na origem e depósito no destino
        await _contaRepository.SacarAsync(contaOrigemId, valor);
        await _contaRepository.DepositarAsync(contaDestinoId, valor);

        // 2. Registra os dois lados no extrato
        await _transacaoRepository.AdicionarAsync(
            new Transacao(valor, "Transferencia Enviada", contaOrigemId));
        await _transacaoRepository.AdicionarAsync(
            new Transacao(valor, "Transferencia Recebida", contaDestinoId));

        _notificador.Notificar(
            $"Transferência de {valor:C} da conta {contaOrigemId} para a conta {contaDestinoId}.");
    }

    /// <summary>Extrato da conta: todos os lançamentos, do mais recente para o mais antigo.</summary>
    /// <exception cref="DominioException">Quando a conta não existe.</exception>
    public async Task<IReadOnlyList<Transacao>> ObterExtratoAsync(int contaId)
    {
        if (!await _contaRepository.ExisteAsync(contaId))
            throw new DominioException($"Conta {contaId} não encontrada.");

        var lancamentos = (await _transacaoRepository.ObterPorContaIdAsync(contaId)).ToList();

        _notificador.Notificar(
            $"Extrato gerado para a conta {contaId}. Total de lançamentos: {lancamentos.Count}.");

        return lancamentos;
    }
}
