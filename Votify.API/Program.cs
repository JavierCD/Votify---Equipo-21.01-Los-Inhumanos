using Microsoft.EntityFrameworkCore;
using Votify.Persistence.Context;
using Votify.Persistence.Repositories;
using Votify.Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. Registrar el contexto de la base de datos (Persistencia)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<VotifyContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Configurar CORS (Para que la Presentación pueda llamar a la REST API)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
        policy.AllowAnyOrigin() // En producción aquí pondrás la URL de tu web
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// 2. Registrar el Repositorio (Inyección de Dependencias)
builder.Services.AddScoped<IVotanteRepository, VotanteRepository>();

// 3. Si tienes un Servicio, regístralo también
// builder.Services.AddScoped<IVotanteService, VotanteService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazor"); // ¡Súper importante! Activa el permiso de comunicación

app.UseAuthorization();

app.MapControllers();

app.Run();
