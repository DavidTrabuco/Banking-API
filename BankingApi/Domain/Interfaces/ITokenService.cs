using BankingApi.Domain.Models;

namespace BankingApi.Domain.Interfaces
{
    public interface ITokenService
    {


    string GetToken(Usuario usuario );
    }
}
