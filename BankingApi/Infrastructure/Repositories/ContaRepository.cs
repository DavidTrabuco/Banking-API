using System.Data;
using BankingApi.Domain.Interfaces;
using BankingApi.Domain.Models;
using Dapper;

namespace BankingApi.Infrastructure.Repositories;

public class ContaRepository : IContaRepository
{
    private readonly IDbConnection _connection;

    public ContaRepository(IDbConnection connection) => _connection = connection;

    public async Task<int> CriarContaAsync(ContaBancaria conta)
    {
        // last_insert_rowid() devolve o Id gerado pelo AUTOINCREMENT do SQLite.
        const string sql = @"
            INSERT INTO Contas (Titular, Saldo, ClienteId, Rua, Cidade, Estado)
            VALUES (@Titular, @Saldo, @ClienteId, @Rua, @Cidade, @Estado);
            SELECT last_insert_rowid();";

        return await _connection.ExecuteScalarAsync<int>(sql, conta);
    }

    public async Task<ContaBancaria?> ObterPorIdAsync(int id)
    {
        const string sql = @"
            SELECT ID, Titular, Saldo, ClienteId, Rua, Cidade, Estado
            FROM Contas
            WHERE ID = @Id";

        return await _connection.QueryFirstOrDefaultAsync<ContaBancaria>(sql, new { Id = id });
    }

    public async Task<bool> ExisteAsync(int id)
    {
        const string sql = "SELECT COUNT(1) FROM Contas WHERE ID = @Id";

        var total = await _connection.ExecuteScalarAsync<int>(sql, new { Id = id });

        return total > 0;
    }

    public Task<bool> SacarAsync(int contaId, decimal valor) =>
        AtualizarSaldoAsync(contaId, conta => conta.Sacar(valor));

    public Task<bool> DepositarAsync(int contaId, decimal valor) =>
        AtualizarSaldoAsync(contaId, conta => conta.Depositar(valor));

    // Carrega a conta, deixa a própria entidade aplicar a regra (Sacar/Depositar)
    // e grava o novo saldo. A regra de negócio fica no domínio, não no SQL.
    private async Task<bool> AtualizarSaldoAsync(int contaId, Func<ContaBancaria, bool> operacao)
    {
        var conta = await ObterPorIdAsync(contaId);
        if (conta is null) return false;

        if (!operacao(conta)) return false;

        const string sqlUpdate = "UPDATE Contas SET Saldo = @Saldo WHERE ID = @Id";

        await _connection.ExecuteAsync(sqlUpdate, new { conta.Saldo, Id = conta.ID });
        return true;
    }
}
