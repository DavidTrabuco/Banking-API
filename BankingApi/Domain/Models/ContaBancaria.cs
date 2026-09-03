namespace BankingApi.Domain.Models;

public class ContaBancaria
{
    public int ID { get; private set; }
    public string Titular { get; private set; } = string.Empty;
    public decimal Saldo { get; private set; }

    // Endereço de cobrança, direto na conta (uma coluna para cada campo).
    public string Rua { get; private set; } = string.Empty;
    public string Cidade { get; private set; } = string.Empty;
    public string Estado { get; private set; } = string.Empty;

    public int ClienteId { get; private set; }
    public Cliente Cliente { get; private set; } = null!;

    public ICollection<Transacao> Transacoes { get; private set; } = new List<Transacao>();

    protected ContaBancaria() { }

    public ContaBancaria(string titular, decimal saldoInicial, string rua, string cidade, string estado, int clienteId)
    {
        Titular = titular;
        Saldo = saldoInicial;
        Rua = rua;
        Cidade = cidade;
        Estado = estado;
        ClienteId = clienteId;
    }

    // Chamado pelo Service logo após o INSERT.
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
