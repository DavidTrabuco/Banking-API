using BankingApi.Interfaces;
using BankingApi.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Adiciona os serviços básicos da Web API ao container do ASP.NET Core
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. REGISTRO DA INJEÇÃO DE DEPENDÊNCIA (DI):
// Diz ao .NET que sempre que alguma classe pedir um "INotificador", 
// ele deve entregar automaticamente uma instância de "NotificadorEmail".
builder.Services.AddScoped<INotificador, NotificadorEmail>();

var app = builder.Build();

// 3. Configura a interface interativa do Swagger para testarmos a API no navegador
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// 4. Mapeia os nossos Controllers para responderem a rotas HTTP (GET, POST, etc.)
app.MapControllers();

app.Run();