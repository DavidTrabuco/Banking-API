using BankingApi.DTO;
using BankingApi.Models;
using BankingApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Controllers
{
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
           
            var cliente = new Cliente(dto.Nome, dto.CPF, dto.Email);

            
            bool sucesso = await _clienteService.CriarClienteAsync(cliente);

            if (!sucesso)
            {
                return BadRequest(new { mensagem = "Falha ao cadastrar cliente (CPF duplicado)." });
            }

            
            return CreatedAtAction(nameof(ObterPorId), new { id = cliente.Id }, cliente);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var cliente = await _clienteService.ObterPorIdAsync(id);

            if (cliente == null)
            {
                return NotFound(new { mensagem = "Cliente não encontrado." });
            }

            return Ok(cliente);
        }
    }
}