namespace BankingApi.DTO
{
    public class CriarContaDTO
    {
        public string Titular { get; set; } = string.Empty;
        public decimal SaldoInicial { get; set; }
        public int ClienteId { get; set; }
        public string Rua { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
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
    
        
    }

    public class TransacaoValorDTO
    {
        public int ContaId { get; set; }
        public decimal Valor { get; set; }
    }

    public class TransferenciaDTO
    {
        public int ContaOrigemId { get; set; }
        public int ContaDestinoId { get; set; }
        public decimal Valor { get; set; }
    }

    public class ClienteDTO
    {
        public string Nome { get; set; } = string.Empty;

        public string CPF { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

    }
