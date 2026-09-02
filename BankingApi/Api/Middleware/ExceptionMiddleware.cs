using System.Text.Json;
using BankingApi.Domain.Exceptions;

namespace BankingApi.Api.Middleware;

/// <summary>
/// Captura exceções em um único lugar, para os controllers não precisarem
/// repetir try/catch em cada action.
/// DominioException  -> 400 Bad Request (erro do cliente da API)
/// qualquer outra    -> 500 Internal Server Error (erro nosso, com log)
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DominioException ex)
        {
            _logger.LogWarning("Regra de negocio violada: {Mensagem}", ex.Message);
            await EscreverRespostaAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro nao tratado ao processar {Metodo} {Caminho}",
                context.Request.Method, context.Request.Path);
            await EscreverRespostaAsync(context, StatusCodes.Status500InternalServerError,
                "Ocorreu um erro inesperado ao processar a requisição.");
        }
    }

    private static Task EscreverRespostaAsync(HttpContext context, int statusCode, string mensagem)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        return context.Response.WriteAsync(JsonSerializer.Serialize(new { mensagem }));
    }
}
