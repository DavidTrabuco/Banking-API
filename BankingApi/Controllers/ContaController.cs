using BankingApi.Data;
using BankingApi.DTO;
using BankingApi.Interfaces;
using BankingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaController : ControllerBase
    {
        private readonly INotificador _notificador;
        private readonly BancoDbContext _context;

        public ContaController(INotificador notificador, BancoDbContext context)
        {
            _notificador = notificador;
            _context = context;
        }

        // POST: api/conta
        [HttpPost]
        public async Task<IActionResult> CriarConta([FromBody] CriarContaDTO dto)
        {
            // Verifica se o cliente realmente existe no banco
            var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == dto.ClienteId);
            if (!clienteExiste)
            {
                return BadRequest(new { mensagem = "Cliente informado não foi encontrado." });
            }

            var endereco = new Endereco(dto.Rua, dto.Cidade, dto.Estado);
            var conta = new ContaBancaria(dto.Titular, dto.SaldoInicial, endereco, dto.ClienteId);

            _context.Contas.Add(conta);
            await _context.SaveChangesAsync();

            _notificador.Notificar($"Conta {conta.ID} criada com sucesso para o titular {conta.Titular}.");

            return CreatedAtAction(nameof(ObterPorId), new { id = conta.ID }, conta);
        }

        // GET: api/conta/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var conta = await _context.Contas
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(c => c.ID == id);

            if (conta == null)
                return NotFound(new { mensagem = "Conta não encontrada." });

            return Ok(conta);
        }

        // GET: api/conta/5/detalhes
        [HttpGet("{id:int}/detalhes")]
        public async Task<IActionResult> ObterDetalhesConta(int id)
        {
            var conta = await _context.Contas.FindAsync(id);

            if (conta == null)
                return NotFound(new { mensagem = "Conta não encontrada." });

            return Ok(new
            {
                Id = conta.ID,
                Titular = conta.Titular,
                Saldo = conta.Saldo,
                EnderecoCobranca = conta.EnderecoCobranca?.ObterEnderecoCompleto()
            });
        }

        // POST: api/conta/sacar
        [HttpPost("sacar")]
        public async Task<IActionResult> Sacar([FromBody] TransacaoValorDTO dto)
        {
            var conta = await _context.Contas.FindAsync(dto.ContaId);

            if (conta == null)
                return NotFound(new { mensagem = "Conta bancária não encontrada." });

            bool sucesso = conta.Sacar(dto.Valor);

            if (!sucesso)
                return BadRequest(new { mensagem = "Saldo insuficiente ou valor de saque inválido." });

            // Cria o registro da transação no histórico
            var transacao = new Transacao(dto.Valor, "Saque", conta.ID);
            _context.Transacoes.Add(transacao);

            // Persiste o novo saldo e a transação no SQLite
            await _context.SaveChangesAsync();

            _notificador.Notificar($"Saque de {dto.Valor:C} realizado na conta {conta.ID}. Novo Saldo: {conta.Saldo:C}");

            return Ok(new
            {
                mensagem = "Saque realizado com sucesso!",
                saldoAtual = conta.Saldo
            });
        }

        // POST: api/conta/depositar
        [HttpPost("depositar")]
        public async Task<IActionResult> Depositar([FromBody] TransacaoValorDTO dto)
        {
            var conta = await _context.Contas.FindAsync(dto.ContaId);

            if (conta == null)
                return NotFound(new { mensagem = "Conta bancária não encontrada." });

            bool sucesso = conta.Depositar(dto.Valor);

            if (!sucesso)
                return BadRequest(new { mensagem = "Valor de depósito inválido." });

            // Registra a transação de depósito
            var transacao = new Transacao(dto.Valor, "Deposito", conta.ID);
            _context.Transacoes.Add(transacao);

            await _context.SaveChangesAsync();

            _notificador.Notificar($"Depósito de {dto.Valor:C} realizado na conta {conta.ID}. Novo Saldo: {conta.Saldo:C}");

            return Ok(new
            {
                mensagem = "Depósito realizado com sucesso!",
                saldoAtual = conta.Saldo
            });
        }

        // GET: api/conta/5/saldo
        [HttpGet("{id:int}/saldo")]
        public async Task<IActionResult> ConsultarSaldo(int id)
        {
            var conta = await _context.Contas.FindAsync(id);

            if (conta == null)
                return NotFound(new { mensagem = "Conta não encontrada." });

            _notificador.Notificar($"Consulta de saldo realizada para a conta {id}.");

            return Ok(new
            {
                contaId = conta.ID,
                saldoAtual = conta.Saldo
            });
        }
    }
}