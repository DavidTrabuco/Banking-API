using System.Data;
using BankingApi.Domain.Interfaces;
using BankingApi.Domain.Models;
using Dapper;

namespace BankingApi.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly IDbConnection _connection;

    public UsuarioRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        
        const string sql = @"
            SELECT 
                Id, 
                Email, 
                SenhaHash, 
                ClienteId 
            FROM Usuarios 
            WHERE Email = @Email";

        return await _connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { Email = email });
    }


    public async Task<Usuario?> RegistrarAsync(Usuario usuario)
    {
        const string sql = @"
            INSERT INTO Usuarios (Email, SenhaHash, ClienteId) 
            VALUES (@Email, @SenhaHash, @ClienteId);
            SELECT last_insert_rowid();";
        var id = await _connection.QuerySingleAsync<int>(sql, new
        {
            Email = usuario.Email,
            SenhaHash = usuario.SenhaHash,
            ClienteId = usuario.ClienteId
        });
        usuario.Id = id;
        return usuario;
    }
}