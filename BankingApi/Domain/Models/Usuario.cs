using static System.Net.Mime.MediaTypeNames;

namespace BankingApi.Domain.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;

    
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

  

    public Usuario() { }

    public Usuario(int id, string nome, string email, string senhaHash, int clienteId)
    {
        Id = id;
        Nome = nome;
        Email = email;
        SenhaHash = senhaHash;
        ClienteId = clienteId;
        
    }
}