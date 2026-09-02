using BankingApi.Domain.Models;

namespace BankingApi.Domain.Interfaces;

public interface ITransacaoRepository
{
    Task AdicionarAsync(Transacao transacao);

    /// <summary>Extrato: todos os lançamentos da conta, do mais recente para o mais antigo.</summary>
    Task<IEnumerable<Transacao>> ObterPorContaIdAsync(int contaId);
}
