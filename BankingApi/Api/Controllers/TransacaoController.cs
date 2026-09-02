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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Depositar([FromBody] TransacaoValorDTO dto)
    {
        await _transacaoService.DepositarAsync(dto.ContaId, dto.Valor);
        return Ok(new { mensagem = "Depósito realizado com sucesso!" });
    }

    [HttpPost("sacar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Sacar([FromBody] TransacaoValorDTO dto)
    {
        await _transacaoService.SacarAsync(dto.ContaId, dto.Valor);
        return Ok(new { mensagem = "Saque realizado com sucesso!" });
    }

    [HttpPost("transferir")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Transferir([FromBody] TransferenciaDTO dto)
    {
        await _transacaoService.TransferirAsync(dto.ContaOrigemId, dto.ContaDestinoId, dto.Valor);
        return Ok(new { mensagem = "Transferência realizada com sucesso!" });
    }

    // Extrato da conta: GET /api/transacao/{contaId}/extrato
    [HttpGet("{contaId:int}/extrato")]
    [ProducesResponseType(typeof(IEnumerable<TransacaoResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ObterExtrato(int contaId)
    {
        var lancamentos = await _transacaoService.ObterExtratoAsync(contaId);

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
