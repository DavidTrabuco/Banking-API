using BankingApi.Application.DTO;
using BankingApi.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankingApi.Domain.Interfaces;
using BankingApi.Infrastructure.Data;
using BankingApi.Application.Services;

namespace BankingApi.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TransacaoController : ControllerBase
    {
        private readonly TransacaoServices _transacaoService;
        private readonly INotificador _notificador;
        private readonly BancoDbContext _context;

        public TransacaoController(TransacaoServices transacao,INotificador notificador, BancoDbContext context)
        {
            _notificador = notificador;
            _context = context;
            _transacaoService = transacao;
        }
        [HttpPost("depositar")]
        public async Task<IActionResult> Depositar([FromBody] TransacaoValorDTO dto)
        {
            
            bool sucesso = await _transacaoService.DepositarAsync(dto.ContaId, dto.Valor);

            if (!sucesso)
                return BadRequest(new { mensagem = "Valor de depósito inválido ou conta não encontrada." });

            _notificador.Notificar($"Depósito de {dto.Valor:C} realizado na conta {dto.ContaId}.");

            return Ok(new { mensagem = "Depósito realizado com sucesso!" });
        }

        [HttpPost("sacar")]
        public async Task<IActionResult> Sacar([FromBody] TransacaoValorDTO dto)
        {
            // O Controller apenas chama a regra de negócio do Service
            bool sucesso = await _transacaoService.SacarAsync(dto.ContaId, dto.Valor);

            if (!sucesso)
                return BadRequest(new { mensagem = "Não foi possível realizar o saque. Verifique a conta e o saldo." });

            _notificador.Notificar($"Saque de {dto.Valor:C} solicitado na conta {dto.ContaId}.");

            return Ok(new { mensagem = "Saque realizado com sucesso!" });
        }

        [HttpPost("transferir")]
        public async Task<IActionResult> Transferir([FromBody] TransferenciaDTO dto)
        {
            bool sucesso = await _transacaoService.TransferirAsync(dto.ContaOrigemId, dto.ContaDestinoId, dto.Valor);

            if (!sucesso)
                return BadRequest(new { mensagem = "Falha na transferência. Verifique os dados das contas e o saldo." });

            _notificador.Notificar($"Transferência de {dto.Valor:C} realizada da conta {dto.ContaOrigemId} para a conta {dto.ContaDestinoId}.");

            return Ok(new { mensagem = "Transferência realizada com sucesso!" });
        }

        [HttpGet("{contaId:int}/extrato")]
        public async Task<IActionResult> ObterExtrato(int contaId)
        {
            var contaExiste = await _context.Contas.AnyAsync(c => c.ID == contaId);

            if (!contaExiste)
                return NotFound(new { mensagem = "Conta bancária não encontrada." });

            var transacoes = await _context.Transacoes
                .Where(t => t.ContaBancariaId == contaId)
                .OrderByDescending(t => t.Id)
                .ToListAsync();

            _notificador.Notificar($"Extrato gerado para a conta {contaId}. Total de lançamentos: {transacoes.Count}");

            return Ok(transacoes);
        }
    }
}
