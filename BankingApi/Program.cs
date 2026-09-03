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
using Microsoft.OpenApi.Models;

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

// Configuração do Swagger com suporte ao JWT (Botão Authorize)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BankingApi", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o token JWT no formato: Bearer {seu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configuração do serviço de Autenticação JWT
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
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// ---------------------------------------------------------------------------
// 2. Acesso a dados
//    EF Core  -> usado APENAS para versionar o schema (dotnet ef migrations).
//    Dapper   -> usado em tempo de execução, através dos Repositories.
// ---------------------------------------------------------------------------
builder.Services.AddDbContext<BancoDbContext>(options => options.UseSqlite(connectionString));

// Uma conexão por requisição, compartilhada pelos três repositórios.
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
// 4. Pipeline HTTP (A ORDEM IMPORTA!)
// ---------------------------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("LiberarFrontend");

// Middlewares de Segurança na ordem correta
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();