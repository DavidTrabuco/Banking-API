using BankingApi.Application.DTO;
using BankingApi.Application.Services;
using BankingApi.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
        var clienteId = ObterClienteIdDoToken();

        if (clienteId is null)
            return Unauthorized(new { mensagem = "Token sem a claim ClienteId." });

        var (status, lancamentos) = await _transacaoService.ObterExtratoAsync(contaId, clienteId.Value);

        // 403 (e não 404) quando a conta existe mas é de outro cliente: o usuário
        // precisa saber que o problema é permissão, não um id inexistente.
        return status switch
        {
            ResultadoExtrato.ContaNaoEncontrada =>
                NotFound(new { mensagem = "Conta bancária não encontrada." }),

            ResultadoExtrato.AcessoNegado =>
                StatusCode(StatusCodes.Status403Forbidden,
                    new { mensagem = "Esta conta não pertence ao usuário autenticado." }),

            _ => Ok(lancamentos.Select(Mapear))
        };
    }

    // O TokenService grava a claim "ClienteId" no JWT emitido no login.
    private int? ObterClienteIdDoToken()
    {
        var valor = User.FindFirst("ClienteId")?.Value;
        return int.TryParse(valor, out var id) ? id : null;
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
