using BankingApi.Domain.Models;

namespace BankingApi.Domain.Interfaces;

public interface ITransacaoRepository
{
    Task AdicionarAsync(Transacao transacao);

    // Extrato: do lançamento mais recente para o mais antigo.
    Task<IEnumerable<Transacao>> ObterPorContaIdAsync(int contaId);
}
