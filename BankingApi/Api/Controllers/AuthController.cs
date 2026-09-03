using BankingApi.Application.DTO;
using BankingApi.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static BankingApi.Application.DTO.LoginDTOs;

namespace BankingApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITokenService _tokenService;

    public AuthController(IUsuarioRepository usuarioRepository, ITokenService tokenService)
    {
        _usuarioRepository = usuarioRepository;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO dto)
    {
        // 1. Busca o usuário no banco pelo e-mail
        var usuario = await _usuarioRepository.ObterPorEmailAsync(dto.Email);

        if (usuario == null)
        {
            return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });
        }

        // 2. Valida se a senha informada bate com a hash salva no banco
        // (Exemplo simples usando BCrypt ou verificação direta se for hash)
        bool senhaValida = BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash);

        if (!senhaValida)
        {
            return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });
        }

        // 3. Gera o Token JWT real com as Claims do usuário achado no banco
        var token = _tokenService.GetToken(usuario);

        // 4. Retorna a resposta com o Token
        return Ok(new LoginResponseDTO(token));
    }
}