using BankingApi.Interfaces;
using BankingApi.Models;
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
            var conta = new ContaBancaria("João Silva", "1000.00", endereco);
            return Ok(new
            {
                Titular = conta.Titular,
                Saldo = conta.Saldo,
                EnderecoCobranca = conta.EnderecoCobranca?.ObterEnderecoCompleto()
            });


        }

        [HttpPost("sacar")]
        public IActionResult Sacar([FromQuery] string valor)
        {
            var endereco = new Endereco("Rua das Flores, 123", "São Paulo", "SP");
            var conta = new ContaBancaria("João Silva", "1000", endereco);

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

    }
}