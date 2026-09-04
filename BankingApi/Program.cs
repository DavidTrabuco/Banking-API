using System.Data;
using System.Text;
using BankingApi.Application.Services;
using BankingApi.Domain.Interfaces;
using BankingApi.Infrastructure.Data;
using BankingApi.Infrastructure.Notifications;
using BankingApi.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// 1. Configuração e CORS
// ---------------------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("Default")?? "Data Source=banco.db";

var frontendUrl = builder.Configuration["FrontendSettings:AllowedUrl"]?? "http://localhost:5173";

builder.Services.AddCors(options =>
{
    options.AddPolicy("LiberarFrontend", policy =>
    {
        policy.WithOrigins(frontendUrl)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Permite o tráfego de cookies HttpOnly entre origens
    });
});

builder.Services.AddControllers();

// Padroniza as rotas em minúsculas (ex: /api/transacao/12/extrato)
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});

// ---------------------------------------------------------------------------
// Autenticação JWT com Leitura de Cookie HttpOnly
// ---------------------------------------------------------------------------
var secretKey = builder.Configuration["Jwt:SecretKey"]!;
var key = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = false,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    // Extrai o token JWT diretamente do Cookie HttpOnly "jwtToken"
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.TryGetValue("jwtToken", out var token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
});

// ---------------------------------------------------------------------------
// 2. Acesso a dados (EF Core & Dapper)
// ---------------------------------------------------------------------------
builder.Services.AddDbContext<BancoDbContext>(options => options.UseSqlite(connectionString));

// Uma conexão por requisição, compartilhada pelos quatro repositórios.
builder.Services.AddScoped<IDbConnection>(_ => new SqliteConnection(connectionString));

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IContaRepository, ContaRepository>();
builder.Services.AddScoped<ITransacaoRepository, TransacaoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// ---------------------------------------------------------------------------
// 3. Camada de aplicação
// ---------------------------------------------------------------------------
builder.Services.AddScoped<INotificador, NotificadorEmail>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<ContaService>();
builder.Services.AddScoped<TransacaoService>();
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

// ---------------------------------------------------------------------------
// 4. Pipeline HTTP
// ---------------------------------------------------------------------------

app.UseHttpsRedirection();

app.UseRouting();

// CORS fica entre Routing e Authentication (ordem exigida pelo ASP.NET Core).
app.UseCors("LiberarFrontend");

// Middlewares de Segurança
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();