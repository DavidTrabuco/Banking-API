namespace BankingApi.Application.DTO
{
    public class LoginDTOs
    {


        public record LoginRequestDTO(string Email, string Senha);
        public record LoginResponseDTO(string Token, string Tipo = "Bearer");
        public record OperacaoContaDTO(decimal Valor);
    }
}
