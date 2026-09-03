using BankingApi.Application.DTO;
using BankingApi.Application.Services;
using BankingApi.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransacaoController : ControllerBase
{
    // O Controller conhece APENAS o Service. Nada de DbContext ou SQL aqui.
    private readonly TransacaoService _transacaoService;

    public TransacaoController(TransacaoService transacaoService)
    {
        _transacaoService = transacaoService;
    }

    [HttpPost("depositar")]
    public async Task<IActionResult> Depositar([FromBody] TransacaoValorDTO dto)
    {
        bool sucesso = await _transacaoService.DepositarAsync(dto.ContaId, dto.Valor);

        if (!sucesso)
            return BadRequest(new { mensagem = "Valor de depósito inválido ou conta não encontrada." });

        return Ok(new { mensagem = "Depósito realizado com sucesso!" });
    }

    [HttpPost("sacar")]
    public async Task<IActionResult> Sacar([FromBody] TransacaoValorDTO dto)
    {
        bool sucesso = await _transacaoService.SacarAsync(dto.ContaId, dto.Valor);

        if (!sucesso)
            return BadRequest(new { mensagem = "Não foi possível realizar o saque. Verifique a conta e o saldo." });

        return Ok(new { mensagem = "Saque realizado com sucesso!" });
    }

    [HttpPost("transferir")]
    public async Task<IActionResult> Transferir([FromBody] TransferenciaDTO dto)
    {
        bool sucesso = await _transacaoService.TransferirAsync(dto.ContaOrigemId, dto.ContaDestinoId, dto.Valor);

        if (!sucesso)
            return BadRequest(new { mensagem = "Falha na transferência. Verifique os dados das contas e o saldo." });

        return Ok(new { mensagem = "Transferência realizada com sucesso!" });
    }

    // Extrato da conta: GET /api/transacao/{contaId}/extrato
    [HttpGet("{contaId:int}/extrato")]
    public async Task<IActionResult> ObterExtrato(int contaId)
    {
        var lancamentos = await _transacaoService.ObterExtratoAsync(contaId);

        if (lancamentos is null)
            return NotFound(new { mensagem = "Conta bancária não encontrada." });

        return Ok(lancamentos.Select(Mapear));
    }

    private static TransacaoResponseDTO Mapear(Transacao transacao) => new()
    {
        Id = transacao.Id,
        Valor = transacao.Valor,
        Tipo = transacao.Tipo,
        Data = transacao.Data,
        ContaId = transacao.ContaBancariaId
    };
}
