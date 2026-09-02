using BankingApi.Application.DTO;
using BankingApi.Application.Services;
using BankingApi.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Api.Controllers;

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
    [ProducesResponseType(typeof(ContaResponseDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CriarConta([FromBody] CriarContaDTO dto)
    {
        var endereco = new Endereco(dto.Rua, dto.Cidade, dto.Estado);
        var conta = new ContaBancaria(dto.Titular, dto.SaldoInicial, endereco, dto.ClienteId);

        // ClienteId inexistente vira DominioException -> 400 pelo middleware.
        await _contaService.CriarContaAsync(conta);

        return CreatedAtAction(nameof(ObterPorId), new { id = conta.ID }, Mapear(conta));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ContaResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        Rua = conta.EnderecoCobranca.Rua,
        Cidade = conta.EnderecoCobranca.Cidade,
        Estado = conta.EnderecoCobranca.Estado
    };
}
