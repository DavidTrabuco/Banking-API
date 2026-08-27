using BankingApi.Data;
using BankingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Services
{
    public class ContaService
    {
        private readonly BancoDbContext _context;

        public ContaService(BancoDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CriarContaAsync(ContaBancaria conta)
        {
           
            var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == conta.ClienteId);
            if (!clienteExiste) return false;

            _context.Contas.Add(conta);
            await _context.SaveChangesAsync(); 
            return true;
        }

        public async Task<ContaBancaria?> ObterPorIdAsync(int id)
        {
            return await _context.Contas.FindAsync(id);
        }
    }
}