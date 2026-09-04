using BankingApi.Domain.Models;

namespace BankingApi.Domain.Interfaces
{
    public interface IUsuarioRepository
    {

        Task<Usuario?> ObterPorEmailAsync(string email);

        Task<Usuario?> RegistrarAsync(Usuario usuario);
    }
}
