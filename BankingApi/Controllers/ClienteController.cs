using BankingApi.Data;
using BankingApi.DTO;
using BankingApi.Interfaces;
using BankingApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly INotificador _notificador;
        private readonly BancoDbContext _context;

        public ClienteController(INotificador notificador, BancoDbContext context)
        {
            _notificador = notificador;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CadastrarCliente([FromBody] ClienteDTO dto)
        {
            var cliente = new Cliente(dto.Nome, dto.CPF, dto.Email);
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            _notificador.Notificar($"Cliente {cliente.Nome} cadastrado com sucesso.");

            return CreatedAtAction(nameof(ObterPorId), new { id = cliente.Id }, cliente);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound(new { mensagem = "Cliente não encontrado." });

            return Ok(cliente);
        }
    }
}