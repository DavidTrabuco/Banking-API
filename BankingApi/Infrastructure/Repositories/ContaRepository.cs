using System.Data;
using BankingApi.Domain.Exceptions;
using BankingApi.Domain.Interfaces;
using BankingApi.Domain.Models;
using Dapper;

namespace BankingApi.Infrastructure.Repositories;

public class ContaRepository : IContaRepository
{
    private readonly IDbConnection _connection;

    public ContaRepository(IDbConnection connection) => _connection = connection;

    /// <summary>Formato "cru" de uma linha da tabela Contas, do jeito que o Dapper consegue ler.</summary>
    private sealed class ContaRow
    {
        public int ID { get; set; }
        public string Titular { get; set; } = string.Empty;
        public decimal Saldo { get; set; }
        public int ClienteId { get; set; }
        public string Rua { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }

    private const string SqlSelecionar = @"
        SELECT ID,
               Titular,
               Saldo,
               ClienteId,
               EnderecoCobranca_Rua    AS Rua,
               EnderecoCobranca_Cidade AS Cidade,
               EnderecoCobranca_Estado AS Estado
        FROM Contas
        WHERE ID = @Id";

    public async Task<int> CriarContaAsync(ContaBancaria conta)
    {
        // O Endereco é um Value Object: no banco ele está "achatado" em três colunas.
        const string sql = @"
            INSERT INTO Contas (Titular, Saldo, ClienteId,
                                EnderecoCobranca_Rua, EnderecoCobranca_Cidade, EnderecoCobranca_Estado)
            VALUES (@Titular, @Saldo, @ClienteId, @Rua, @Cidade, @Estado);
            SELECT last_insert_rowid();";

        return await _connection.ExecuteScalarAsync<int>(
            sql,
            new
            {
                conta.Titular,
                conta.Saldo,
                conta.ClienteId,
                conta.EnderecoCobranca.Rua,
                conta.EnderecoCobranca.Cidade,
                conta.EnderecoCobranca.Estado
            });
    }

    public async Task<ContaBancaria?> ObterPorIdAsync(int id)
    {
        var row = await _connection.QueryFirstOrDefaultAsync<ContaRow>(SqlSelecionar, new { Id = id });

        if (row is null) return null;

        return ContaBancaria.Restaurar(
            row.ID,
            row.Titular,
            row.Saldo,
            new Endereco(row.Rua, row.Cidade, row.Estado),
            row.ClienteId);
    }

    public async Task<bool> ExisteAsync(int id)
    {
        const string sql = "SELECT COUNT(1) FROM Contas WHERE ID = @Id";

        var total = await _connection.ExecuteScalarAsync<int>(sql, new { Id = id });

        return total > 0;
    }

    public Task SacarAsync(int contaId, decimal valor) =>
        AtualizarSaldoAsync(contaId, conta => conta.Sacar(valor),
            "Saldo insuficiente ou valor de saque inválido.");

    public Task DepositarAsync(int contaId, decimal valor) =>
        AtualizarSaldoAsync(contaId, conta => conta.Depositar(valor),
            "Valor de depósito inválido.");

    /// <summary>
    /// Carrega a conta, deixa a PRÓPRIA entidade aplicar a regra (Sacar/Depositar)
    /// e grava o novo saldo. A regra de negócio fica no domínio, não no SQL.
    /// </summary>
    private async Task AtualizarSaldoAsync(int contaId, Func<ContaBancaria, bool> operacao, string mensagemErro)
    {
        var conta = await ObterPorIdAsync(contaId)
            ?? throw new DominioException($"Conta {contaId} não encontrada.");

        if (!operacao(conta))
            throw new DominioException(mensagemErro);

        const string sqlUpdate = "UPDATE Contas SET Saldo = @Saldo WHERE ID = @Id";

        await _connection.ExecuteAsync(sqlUpdate, new { conta.Saldo, Id = conta.ID });
    }
}
