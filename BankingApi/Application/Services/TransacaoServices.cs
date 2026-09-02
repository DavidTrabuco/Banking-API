using BankingApi.Infrastructure.Data;
using BankingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Services;

public class TransacaoServices
{
    private readonly BancoDbContext _context;

    public TransacaoServices(BancoDbContext context)
    {
        _context = context;
    }

    public async Task<bool> SacarAsync(int contaId, decimal valor)
    {
        var conta = await _context.Contas.FindAsync(contaId);
        if (conta == null) return false;

        bool sucesso = conta.Sacar(valor);
        if (!sucesso) return false;

        
        var transacao = new Transacao(valor, "Saque", conta.ID);
        _context.Transacoes.Add(transacao);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DepositarAsync(int contaId, decimal valor)
    {
        var conta = await _context.Contas.FindAsync(contaId);
        if (conta == null) return false;

        bool sucesso = conta.Depositar(valor);
        if (!sucesso) return false;

        var transacao = new Transacao(valor, "Deposito", conta.ID);
        _context.Transacoes.Add(transacao);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TransferirAsync(int contaOrigemId, int contaDestinoId, decimal valor)
    {
        var contaOrigem = await _context.Contas.FindAsync(contaOrigemId);
        var contaDestino = await _context.Contas.FindAsync(contaDestinoId);

        if (contaOrigem == null || contaDestino == null) return false;

        bool sacou = contaOrigem.Sacar(valor);
        if (!sacou) return false;

        contaDestino.Depositar(valor);

     
        var transacaoSaque = new Transacao(valor, "Transferencia Enviada", contaOrigem.ID);
        var transacaoDeposito = new Transacao(valor, "Transferencia Recebida", contaDestino.ID);

        _context.Transacoes.AddRange(transacaoSaque, transacaoDeposito);

        await _context.SaveChangesAsync();
        return true;
    }
}