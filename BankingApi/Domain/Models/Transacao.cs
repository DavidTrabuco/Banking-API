namespace BankingApi.Domain.Models;

public class Transacao
{
    public int Id { get; private set; }
    public decimal Valor { get; private set; }
    public string Tipo { get; private set; } = string.Empty;
    public DateTime Data { get; private set; } = DateTime.Now;

    public int ContaBancariaId { get; private set; }
    public ContaBancaria ContaBancaria { get; private set; } = null!;

    protected Transacao() { }

    public Transacao(decimal valor, string tipo, int contaBancariaId)
    {
        Valor = valor;
        Tipo = tipo;
        ContaBancariaId = contaBancariaId;
    }
}