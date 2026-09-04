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

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,                  // Impede leitura via JavaScript (XSS)
            Secure = true,                    // Exige HTTPS
            SameSite = SameSiteMode.None,     // Permite requisições entre origens (ex: React 5173 e API 5043)
            Expires = DateTime.UtcNow.AddHours(8)
        };

        // 4. Grava o cookie "jwtToken" no cabeçalho da resposta HTTP
        Response.Cookies.Append("jwtToken", token, cookieOptions);

        // 5. Retorna 200 OK (não precisa mais enviar o token no corpo do JSON!)
        return Ok(new { mensagem = "Login realizado com sucesso!" });
    }



    [HttpPost("registrar")]
    [AllowAnonymous]

    public async Task<IActionResult> Registrar([FromBody] RegistroRequestDTO dto)
    {
        // 1. Verifica se o e-mail já está registrado
        var usuarioExistente = await _usuarioRepository.ObterPorEmailAsync(dto.Email);
        if (usuarioExistente != null)
        {
            return BadRequest(new { mensagem = "E-mail já registrado." });
        }
        // 2. Cria um novo usuário com a senha hash
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(dto.SenhaHash);
        var novoUsuario = new Domain.Models.Usuario
        {
            Email = dto.Email,
            SenhaHash = senhaHash,
            ClienteId = dto.ClienteId
        };
        // 3. Salva o usuário no banco
        var usuarioRegistrado = await _usuarioRepository.RegistrarAsync(novoUsuario);

        // RegistrarAsync devolve Usuario? -- sem esta guarda o acesso abaixo
        // dispara CS8602 (possível referência nula).
        if (usuarioRegistrado is null)
            return BadRequest(new { mensagem = "Não foi possível registrar o usuário." });

        // 4. Retorna a resposta com os dados do usuário registrado (sem a senha).
        //    Nao usamos CreatedAtAction: a action Login e um POST sem parametro {id}
        //    na rota, entao o ASP.NET nao consegue gerar o header Location e lanca.
        return Ok(new
        {
            usuarioRegistrado.Id,
            usuarioRegistrado.Email,
            usuarioRegistrado.ClienteId
        });
    }
}