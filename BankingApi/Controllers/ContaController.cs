using BankingApi.Interfaces;
using BankingApi.Models;
using BankingApi.DTO;
using Microsoft.AspNetCore.Mvc;


namespace BankingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaController : Controller
    {
        private readonly INotificador _notificador;

        public ContaController(INotificador notificador)
        {
            _notificador = notificador;
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
        public IActionResult Sacar([FromQuery] decimal valor)
        {
            var endereco = new Endereco("Rua das Flores, 123", "São Paulo", "SP");
            var conta = new ContaBancaria("João Silva", 1000m, endereco);

            bool sucesso = conta.Sacar(valor);

            if (!sucesso)
            {
                return BadRequest(new { mensagem = "Saldo insuficiente para realizar o saque." });
            }

            // Executa a notificação injetada via DI
            _notificador.Notificar($"Saque de {valor:C} realizado com sucesso. Saldo restante: {conta.Saldo:C}");

            return Ok(new
            {
                mensagem = "Saque realizado com sucesso!",
                saldoAtual = conta.Saldo
            });
        }



        [HttpPost("depositar")]

        public IActionResult Depositar([FromBody] DepositarDTO dto)
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

            return Ok(new
            {
                saldoAtual = saldo
            });
        }
    }
}