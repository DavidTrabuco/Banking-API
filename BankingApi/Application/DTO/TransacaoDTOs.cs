using System.ComponentModel.DataAnnotations;

namespace BankingApi.Application.DTO;

public class TransacaoValorDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "Informe uma ContaId válida.")]
    public int ContaId { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
    public decimal Valor { get; set; }
}

public class TransferenciaDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "Informe uma ContaOrigemId válida.")]
    public int ContaOrigemId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Informe uma ContaDestinoId válida.")]
    public int ContaDestinoId { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
    public decimal Valor { get; set; }
}

// Uma linha do extrato. Não devolvemos a entidade Transacao direto porque ela
// carrega a navegação ContaBancaria, o que geraria referência circular no JSON.
public class TransacaoResponseDTO
{
    public int Id { get; set; }
    public decimal Valor { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public int ContaId { get; set; }
}
