using System.Data;
using BankingApi.Api.Middleware;
using BankingApi.Application.Services;
using BankingApi.Domain.Interfaces;
using BankingApi.Infrastructure.Data;
using BankingApi.Infrastructure.Notifications;
using BankingApi.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// 1. Configuração
// ---------------------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("Default")
                       ?? "Data Source=banco.db";

var frontendUrl = builder.Configuration["FrontendSettings:AllowedUrl"]
                  ?? "http://localhost:5173";

builder.Services.AddCors(options =>
{
    options.AddPolicy("LiberarFrontend", policy =>
    {
        policy.WithOrigins(frontendUrl)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---------------------------------------------------------------------------
// 2. Acesso a dados
//    EF Core  -> usado APENAS para versionar o schema (dotnet ef migrations).
//    Dapper   -> usado em tempo de execução, através dos Repositories.
// ---------------------------------------------------------------------------
builder.Services.AddDbContext<BancoDbContext>(options => options.UseSqlite(connectionString));

// Uma conexão por requisição, compartilhada pelos três repositórios.
// O Dapper abre e fecha a conexão sozinho a cada comando.
builder.Services.AddScoped<IDbConnection>(_ => new SqliteConnection(connectionString));

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IContaRepository, ContaRepository>();
builder.Services.AddScoped<ITransacaoRepository, TransacaoRepository>();

// ---------------------------------------------------------------------------
// 3. Camada de aplicação
// ---------------------------------------------------------------------------
builder.Services.AddScoped<INotificador, NotificadorEmail>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<ContaService>();
builder.Services.AddScoped<TransacaoService>();

var app = builder.Build();

// ---------------------------------------------------------------------------
// 4. Pipeline HTTP (A ORDEM IMPORTA!)
// ---------------------------------------------------------------------------

// Primeiro de todos: qualquer exceção lançada abaixo passa por aqui na volta.
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS precisa vir ANTES de UseAuthorization e MapControllers.
app.UseCors("LiberarFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
