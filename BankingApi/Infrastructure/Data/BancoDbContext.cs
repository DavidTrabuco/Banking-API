using BankingApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Infrastructure.Data;

public class BancoDbContext : DbContext
{
    public BancoDbContext(DbContextOptions<BancoDbContext> options) : base(options) { }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<ContaBancaria> Contas => Set<ContaBancaria>();
    public DbSet<CartaoCredito> Cartoes => Set<CartaoCredito>();
    public DbSet<Transacao> Transacoes => Set<Transacao>();

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relacionamento 1:N -> Cliente possui Varias Contas
        modelBuilder.Entity<ContaBancaria>()
            .HasOne(c => c.Cliente)
            .WithMany(cl => cl.Contas)
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento 1:N -> Cliente possui Varios Cartoes
        modelBuilder.Entity<CartaoCredito>()
            .HasOne(c => c.Cliente)
            .WithMany(cl => cl.Cartoes)
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento 1:N -> ContaBancaria possui Varias Transacoes
        modelBuilder.Entity<Transacao>()
            .HasOne(t => t.ContaBancaria)
            .WithMany(c => c.Transacoes)
            .HasForeignKey(t => t.ContaBancariaId)
            .OnDelete(DeleteBehavior.Cascade);
        //Relacionamento 1:1 -> Usuario possui 1 Cliente
        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Cliente)
            .WithOne(c => c.Usuario)
            .HasForeignKey<Usuario>(u => u.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);


    }
}