using System.Data;
using BankingApi.Domain.Interfaces;
using BankingApi.Domain.Models;
using Dapper;

namespace BankingApi.Infrastructure.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly IDbConnection _connection;

    public ClienteRepository(IDbConnection connection) => _connection = connection;

    public async Task<bool> ExisteCpfAsync(string cpf)
    {
        const string sql = "SELECT COUNT(1) FROM Clientes WHERE Cpf = @Cpf";

        var total = await _connection.ExecuteScalarAsync<int>(sql, new { Cpf = cpf });

        return total > 0;
    }

    public async Task<int> AdicionarAsync(Cliente cliente)
    {
        // last_insert_rowid() devolve o Id gerado pelo AUTOINCREMENT do SQLite.
        const string sql = @"
            INSERT INTO Clientes (Nome, Cpf, Email)
            VALUES (@Nome, @Cpf, @Email);
            SELECT last_insert_rowid();";

        return await _connection.ExecuteScalarAsync<int>(
            sql, new { cliente.Nome, cliente.Cpf, cliente.Email });
    }

    public async Task<Cliente?> ObterPorIdAsync(int id)
    {
        // Colunas explícitas em vez de SELECT * : se a tabela mudar, o erro
        // aparece aqui e não silenciosamente em uma propriedade nula.
        const string sql = "SELECT Id, Nome, Cpf, Email FROM Clientes WHERE Id = @Id";

        return await _connection.QueryFirstOrDefaultAsync<Cliente>(sql, new { Id = id });
    }
}
