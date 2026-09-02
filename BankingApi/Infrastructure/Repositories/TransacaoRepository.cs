using System.Data;
using BankingApi.Domain.Interfaces;
using BankingApi.Domain.Models;
using Dapper;

namespace BankingApi.Infrastructure.Repositories;

public class TransacaoRepository : ITransacaoRepository
{
    private readonly IDbConnection _connection;

    public TransacaoRepository(IDbConnection connection) => _connection = connection;

    public async Task AdicionarAsync(Transacao transacao)
    {
        // A coluna da FK chama-se ContaBancariaId (foi assim que a migration a criou).
        const string sql = @"
            INSERT INTO Transacoes (Valor, Tipo, Data, ContaBancariaId)
            VALUES (@Valor, @Tipo, @Data, @ContaBancariaId)";

        await _connection.ExecuteAsync(
            sql,
            new { transacao.Valor, transacao.Tipo, transacao.Data, transacao.ContaBancariaId });
    }

    public async Task<IEnumerable<Transacao>> ObterPorContaIdAsync(int contaId)
    {
        const string sql = @"
            SELECT Id, Valor, Tipo, Data, ContaBancariaId
            FROM Transacoes
            WHERE ContaBancariaId = @ContaId
            ORDER BY Id DESC";

        return await _connection.QueryAsync<Transacao>(sql, new { ContaId = contaId });
    }
}
