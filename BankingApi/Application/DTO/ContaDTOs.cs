using System.ComponentModel.DataAnnotations;

namespace BankingApi.Application.DTO;

public class CriarContaDTO
{
    [Required(ErrorMessage = "O titular é obrigatório.")]
    public string Titular { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "O saldo inicial não pode ser negativo.")]
    public decimal SaldoInicial { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Informe um ClienteId válido.")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "A rua é obrigatória.")]
    public string Rua { get; set; } = string.Empty;

    [Required(ErrorMessage = "A cidade é obrigatória.")]
    public string Cidade { get; set; } = string.Empty;

    [Required(ErrorMessage = "O estado é obrigatório.")]
    public string Estado { get; set; } = string.Empty;
}

public class ContaResponseDTO
{
    public int Id { get; set; }
    public string Titular { get; set; } = string.Empty;
    public decimal Saldo { get; set; }
    public int ClienteId { get; set; }
    public string Rua { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}
