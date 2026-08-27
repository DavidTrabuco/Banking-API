using BankingApi.DTO;
using BankingApi.Models;
using BankingApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaController : ControllerBase
    {
        private readonly ContaService _contaService;

        public ContaController(ContaService contaService)
        {
            _contaService = contaService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarConta([FromBody] CriarContaDTO dto)
        {
            var endereco = new Endereco(dto.Rua, dto.Cidade, dto.Estado);

            var conta = new ContaBancaria(dto.Titular, dto.SaldoInicial, endereco, dto.ClienteId);

            bool sucesso = await _contaService.CriarContaAsync(conta);

            if (!sucesso)
            {
                return BadRequest(new { mensagem = "Falha ao criar conta. O ClienteId informado não existe." });
            }

           
            var response = new ContaResponseDTO
            {
                Id = conta.ID,
                Titular = conta.Titular,
                Saldo = conta.Saldo,
                ClienteId = conta.ClienteId,
                Rua = conta.EnderecoCobranca.Rua,
                Cidade = conta.EnderecoCobranca.Cidade,
                Estado = conta.EnderecoCobranca.Estado
            };

            return CreatedAtAction(nameof(ObterPorId), new { id = conta.ID }, response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var conta = await _contaService.ObterPorIdAsync(id);

            if (conta == null)
            {
                return NotFound(new { mensagem = "Conta bancária não encontrada." });
            }

            var response = new ContaResponseDTO
            {
                Id = conta.ID,
                Titular = conta.Titular,
                Saldo = conta.Saldo,
                ClienteId = conta.ClienteId,
                Rua = conta.EnderecoCobranca.Rua,
                Cidade = conta.EnderecoCobranca.Cidade,
                Estado = conta.EnderecoCobranca.Estado
            };

            return Ok(response);
        }
    }
}