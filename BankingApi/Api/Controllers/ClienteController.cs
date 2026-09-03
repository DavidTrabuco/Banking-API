using BankingApi.Application.DTO;
using BankingApi.Application.Services;
using BankingApi.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly ClienteService _clienteService;

    public ClienteController(ClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpPost]
    public async Task<IActionResult> CadastrarCliente([FromBody] ClienteDTO dto)
    {
        var cliente = new Cliente(dto.Nome, dto.Cpf, dto.Email);

        bool sucesso = await _clienteService.CriarClienteAsync(cliente);

        if (!sucesso)
            return BadRequest(new { mensagem = "Falha ao cadastrar cliente (CPF duplicado)." });

        return CreatedAtAction(nameof(ObterPorId), new { id = cliente.Id }, Mapear(cliente));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var cliente = await _clienteService.ObterPorIdAsync(id);

        if (cliente is null)
            return NotFound(new { mensagem = "Cliente não encontrado." });

        return Ok(Mapear(cliente));
    }

    private static ClienteResponseDTO Mapear(Cliente cliente) => new()
    {
        Id = cliente.Id,
        Nome = cliente.Nome,
        Cpf = cliente.Cpf,
        Email = cliente.Email
    };
}
