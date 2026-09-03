using BankingApi.Application.DTO;
using BankingApi.Application.Services;
using BankingApi.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
        var conta = new ContaBancaria(
            dto.Titular, dto.SaldoInicial, dto.Rua, dto.Cidade, dto.Estado, dto.ClienteId);

        bool sucesso = await _contaService.CriarContaAsync(conta);

        if (!sucesso)
            return BadRequest(new { mensagem = "Falha ao criar conta. O ClienteId informado não existe." });

        return CreatedAtAction(nameof(ObterPorId), new { id = conta.ID }, Mapear(conta));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var conta = await _contaService.ObterPorIdAsync(id);

        if (conta is null)
            return NotFound(new { mensagem = "Conta bancária não encontrada." });

        return Ok(Mapear(conta));
    }

    private static ContaResponseDTO Mapear(ContaBancaria conta) => new()
    {
        Id = conta.ID,
        Titular = conta.Titular,
        Saldo = conta.Saldo,
        ClienteId = conta.ClienteId,
        Rua = conta.Rua,
        Cidade = conta.Cidade,
        Estado = conta.Estado
    };
}
