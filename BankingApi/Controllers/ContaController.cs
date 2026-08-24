using BankingApi.Interfaces;
using BankingApi.Models;
using BankingApi.DTO;
using Microsoft.AspNetCore.Mvc;
using BankingApi.Data;

namespace BankingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaController : Controller
    {
        private readonly INotificador _notificador;
        private readonly BancoDbContext _context;

        
        public ContaController(INotificador notificador, BancoDbContext context)
        {
            _notificador = notificador;
            _context = context;
        }

       

        [HttpGet("detalhes")]
        public IActionResult ObterDetalhesConta()
        {
            var endereco = new Endereco("Rua Exemplo", "Cidade Exemplo", "Estado Exemplo");
            var conta = new ContaBancaria("João Silva", 1000m, endereco);
            return Ok(new
            {
                Titular = conta.Titular,
                Saldo = conta.Saldo,
                EnderecoCobranca = conta.EnderecoCobranca?.ObterEnderecoCompleto()
            });
        }

        [HttpPost("sacar")]
        public IActionResult Sacar([FromBody] ValorDTO dto)
        {
            var endereco = new Endereco("Rua das Flores, 123", "São Paulo", "SP");
            var conta = new ContaBancaria("João Silva", 1000m, endereco);

            bool sucesso = conta.Sacar(dto.Valor);

            if (!sucesso)
            {
                return BadRequest(new { mensagem = "Saldo insuficiente para realizar o saque." });
            }

            _notificador.Notificar($"Saque de {dto.Valor:C} realizado com sucesso. Saldo restante: {conta.Saldo:C}");

            return Ok(new
            {
                mensagem = "Saque realizado com sucesso!",
                saldoAtual = conta.Saldo
            });
        }

        [HttpPost("depositar")]
        public IActionResult Depositar([FromBody] ValorDTO dto)
        {
            var endereco = new Endereco("Rua das Flores, 123", "São Paulo", "SP");
            var conta = new ContaBancaria("João Silva", 1000m, endereco);
            conta.Depositar(dto.Valor);

            _notificador.Notificar($"Depósito de {dto.Valor:C} realizado com sucesso. Saldo atual: {conta.Saldo:C}");
            return Ok(new
            {
                mensagem = "Depósito realizado com sucesso!",
                saldoAtual = conta.Saldo
            });
        }

        [HttpGet("ConsultarSaldo")]
        public IActionResult ConsultarSaldo()
        {
            var endereco = new Endereco("Rua das Flores, 123", "São Paulo", "SP");
            var conta = new ContaBancaria("João Silva", 1000m, endereco);
            var saldo = conta.Saldo;
            _notificador.Notificar($"Consulta de saldo realizada. Saldo atual: {conta.Saldo:C}");

            return Ok(new
            {
                saldoAtual = saldo
            });
        }

        [HttpPost]
        public IActionResult CriarConta([FromBody] ContaBancaria conta)
        {
            _context.Contas.Add(conta);
            _context.SaveChanges();
            return CreatedAtAction(nameof(ObterDetalhesConta), new { id = conta.ID }, conta);
        }

        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            var conta = _context.Contas.Find(id);

            if (conta == null)
                return NotFound("Conta não encontrada.");

            return Ok(conta);
        }
    }
}