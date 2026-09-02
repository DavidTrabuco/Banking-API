namespace BankingApi.Domain.Models;

/// <summary>
/// Value Object: não tem Id próprio, é sempre parte de uma ContaBancaria.
/// No banco vira as colunas EnderecoCobranca_Rua / _Cidade / _Estado (ver BancoDbContext).
/// </summary>
public class Endereco
{
    public string Rua { get; private set; } = string.Empty;
    public string Cidade { get; private set; } = string.Empty;
    public string Estado { get; private set; } = string.Empty;

    // Construtor sem parâmetros exigido pelo EF Core para materializar a entidade.
    protected Endereco() { }

    public Endereco(string rua, string cidade, string estado)
    {
        Rua = rua;
        Cidade = cidade;
        Estado = estado;
    }

    public string ObterEnderecoCompleto() => $"{Rua}, {Cidade}, {Estado}";
}
