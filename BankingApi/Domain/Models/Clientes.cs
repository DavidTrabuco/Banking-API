namespace BankingApi.Domain.Models;

public class Cliente
{
    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Cpf { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    public ICollection<ContaBancaria> Contas { get; private set; } = new List<ContaBancaria>();
    public ICollection<CartaoCredito> Cartoes { get; private set; } = new List<CartaoCredito>();

    protected Cliente() { }

    public Cliente(string nome, string cpf, string email)
    {
        Nome = nome;
        Cpf = cpf;
        Email = email;
    }

    // Chamado pelo Service logo após o INSERT.
    public void DefinirId(int id) => Id = id;
}
