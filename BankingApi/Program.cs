using BankingApi.Domain.Interfaces;
using BankingApi.Infrastructure.Data;
using BankingApi.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurações de Serviços e Dependências
var frontendUrl = builder.Configuration["FrontendSettings:AllowedUrl"] ?? "http://localhost:5173";

builder.Services.AddCors(options =>
{
    options.AddPolicy("LiberarFrontend", policy =>
    {
        policy.WithOrigins(frontendUrl!)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<BancoDbContext>(options => options.UseSqlite("Data Source=banco.db"));

// 2. Registro da Injeção de Dependência (DI)
builder.Services.AddScoped<INotificador, NotificadorEmail>();
builder.Services.AddScoped<TransacaoServices>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<ContaService>();

var app = builder.Build();

// 3. Middlewares do Pipeline HTTP (A ORDEM IMPORTA!)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// O CORS DEVE vir ANTES do UseAuthorization e do MapControllers
app.UseCors("LiberarFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();