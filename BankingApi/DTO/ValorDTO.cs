namespace BankingApi.DTO
{
    // DTO genérico usado como "envelope" para transferir apenas um valor numérico decimal.
    // Pode ser reutilizado em qualquer operação que exija apenas um valor (ex: Depositar, Sacar).
    public class ValorDTO
    {
        // get e set públicos são obrigatórios para o .NET ler o JSON do Swagger
        // e preencher o dado automaticamente.
        public decimal Valor { get; set; }
    }
}