using BankingApi.Data;
using BankingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Services
{
    public class ClienteService
    {
        private readonly BancoDbContext _context;

        public ClienteService(BancoDbContext context)
        {
            _context = context;
        }

       
        public async Task<bool> CriarClienteAsync(Cliente cliente)
        {
            var cpfExiste = await _context.Clientes.AnyAsync(c => c.CPF == cliente.CPF);
            if (cpfExiste) return false;

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync(); 
            return true;
        }

        
        public async Task<Cliente?> ObterPorIdAsync(int id)
        {
            return await _context.Clientes.FindAsync(id);
        }
    }
}