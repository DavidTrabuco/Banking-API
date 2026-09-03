namespace BankingApi.Application.DTO
{
    public class LoginResponseDTOs
    {

        public string Token { get; init; } = string.Empty;
        public string Tipo { get; init; } = "Bearer";
        public LoginResponseDTOs(string token, string tipo)
        {
            Token = token;
            Tipo = tipo;
        }
    }
}
