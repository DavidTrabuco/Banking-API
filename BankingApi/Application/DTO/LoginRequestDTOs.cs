using System.Reflection.Metadata;

namespace BankingApi.Application.DTO
{
    public class LoginRequestDTOs
    {


        public string Email { get; init; } = string.Empty;
        public string Senha { get; init; } = string.Empty;


        public LoginRequestDTOs (string email, string senha)
        {
            Email = email;
            Senha = senha;
        }
    }
}
