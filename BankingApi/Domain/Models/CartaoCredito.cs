namespace BankingApi.Domain.Models;

public class CartaoCredito
{
    public int Id { get; private set; }
    public string NumeroCartao { get; private set; } = string.Empty;
    public decimal LimiteTotal { get; private set; }
    public decimal LimiteDisponivel { get; private set; }
    public int DiaVencimento { get; private set; }

    public int ClienteId { get; private set; }
    public Cliente Cliente { get; private set; } = null!;

    protected CartaoCredito() { }

    public CartaoCredito(string numeroCartao, decimal limite, int diaVencimento, int clienteId)
    {
        NumeroCartao = numeroCartao;
        LimiteTotal = limite;
        LimiteDisponivel = limite;
        DiaVencimento = diaVencimento;
        ClienteId = clienteId;
    }
}
