namespace BankingApi.Domain.Models;

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

    /// <summary>
    /// Reconstrói uma conta já existente a partir das linhas do banco.
    /// O Dapper não consegue montar sozinho o objeto aninhado EnderecoCobranca,
    /// então o repositório usa esta fábrica em vez de mexer nos setters privados.
    /// </summary>
    public static ContaBancaria Restaurar(int id, string titular, decimal saldo, Endereco endereco, int clienteId)
        => new(titular, saldo, endereco, clienteId) { ID = id };

    /// <summary>Define o Id devolvido pelo banco logo após o INSERT.</summary>
    public void DefinirId(int id) => ID = id;

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
