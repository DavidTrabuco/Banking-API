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

        [HttpPost]
        public async Task<IActionResult> CriarConta([FromBody] CriarContaDTO dto)
        {
          
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

        
        [HttpGet("{id:int}/detalhes")]
        public async Task<IActionResult> ObterDetalhesConta(int id)
        {
            var conta = await _context.Contas
                .Include(c => c.EnderecoCobranca)
                .FirstOrDefaultAsync(c => c.ID == id);

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