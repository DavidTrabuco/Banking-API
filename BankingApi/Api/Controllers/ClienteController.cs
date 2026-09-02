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
    [ProducesResponseType(typeof(ClienteResponseDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CadastrarCliente([FromBody] ClienteDTO dto)
    {
        var cliente = new Cliente(dto.Nome, dto.Cpf, dto.Email);

        // Se o CPF ja existir, o Service lanca DominioException e o
        // ExceptionMiddleware devolve 400 automaticamente.
        await _clienteService.CriarClienteAsync(cliente);

        return CreatedAtAction(nameof(ObterPorId), new { id = cliente.Id }, Mapear(cliente));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClienteResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
