namespace BankingApi.Models;

public class Cliente
{
    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string CPF { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    public ICollection<ContaBancaria> Contas { get; private set; } = new List<ContaBancaria>();
    public ICollection<CartaoCredito> Cartoes { get; private set; } = new List<CartaoCredito>();

    protected Cliente() { }

    public Cliente(string nome, string cpf, string email)
    {
        Nome = nome;
        CPF = cpf;
        Email = email;
    }
}