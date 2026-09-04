using System.Reflection.Metadata;

namespace BankingApi.Application.DTO
{
    public class RegistroRequestDTO
    {

        public string Email { get; init; } = string.Empty;

        public string SenhaHash { get; init; } = string.Empty;

        public int ClienteId { get; init; }

        public RegistroRequestDTO(string email, string senhaHash, int clienteId)
        {
            Email = email;
            SenhaHash = senhaHash;
            ClienteId = clienteId;
        }
    }
}
