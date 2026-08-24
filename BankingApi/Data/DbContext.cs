using BankingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Data;

// O garçom precisa herdar de 'DbContext' para aprender as funções de garçom do EF Core
public class BancoDbContext : DbContext
{
    // O construtor recebe as opções de configuração (como qual banco usar)
    public BancoDbContext(DbContextOptions<BancoDbContext> options) : base(options)
    {
    }

    // O DbSet é a "seção do cardápio/caderneta" que o garçom gerencia.
    // Aqui dizemos que ele vai cuidar de uma tabela de 'ContaBancaria' chamada 'Contas'.
    public DbSet<ContaBancaria> Contas { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContaBancaria>()
            .OwnsOne(c => c.EnderecoCobranca);
    }
}