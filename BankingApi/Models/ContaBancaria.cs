namespace BankingApi.Models;

public class ContaBancaria
{
    public int ID { get; private set; }
    public string Titular { get; private set; } = string.Empty;
    public decimal Saldo { get; private set; }
    public Endereco EnderecoCobranca { get; private set; } = null!;

    public int ClienteId { get; private set; }
    public Cliente Cliente { get; private set; } = null!;

    public ICollection<Transacao> Transacoes { get; private set; } = new List<Transacao>();

    protected ContaBancaria() { }

    
    public ContaBancaria(string titular, decimal saldoInicial, Endereco endereco, int clienteId)
    {
        Titular = titular;
        Saldo = saldoInicial;
        EnderecoCobranca = endereco;
        ClienteId = clienteId;
    }

    public bool Sacar(decimal valor)
    {
        if (valor <= 0 || valor > Saldo) return false;
        Saldo -= valor;
        return true;
    }

    
    public bool Depositar(decimal valor)
    {
        if (valor <= 0) return false;
        Saldo += valor;
        return true;
    }
}