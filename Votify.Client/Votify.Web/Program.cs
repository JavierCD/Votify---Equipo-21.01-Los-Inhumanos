using Microsoft.EntityFrameworkCore;
using Votify.Persistence.Context;
using Votify.Persistence.Repositories;
using Votify.Core.Interfaces;
using Votify.UI;
using Votify.Web.Components;
using Radzen;
using Votify.Services.Implementations;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. CONFIGURACIÓN DE SERVICIOS (CONTENEDOR)
// ==========================================

// --- Blazor ---
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddRadzenComponents();

// --- API y Swagger ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Base de Datos (PostgreSQL) ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<VotifyContext>(options =>
    options.UseNpgsql(connectionString));

// --- CORS (Para que UI y Web se comuniquen) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// --- Inyección de Dependencias (Core, Persistence, Services) ---
// Registrar el Repositorio Genérico para todas las entidades
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
// builder.Services.AddScoped<IVotanteService, VotanteService>(); // Descomenta cuando lo necesites
// 2. Registramos el servicio de Eventos que acabamos de crear
builder.Services.AddScoped<IEventoService, EventoService>();


// ==========================================
// 2. CONFIGURACIÓN DEL PIPELINE (MIDDLEWARE)
// ==========================================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging(); // Debug para Blazor WASM
    app.UseSwagger();              // Documentación de la API
    app.UseSwaggerUI();            // Interfaz visual de Swagger
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// ¡CORS debe ir siempre antes de Antiforgery y Authorization!
app.UseCors("AllowBlazor");

app.UseAntiforgery();
app.UseAuthorization();

// ==========================================
// 3. MAPEO DE RUTAS (ENDPOINTS)
// ==========================================

// Mapea las rutas de tu API (ej: /api/votantes)
app.MapControllers();

// Mapea las páginas de Blazor
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Votify.UI._Imports).Assembly);

// INICIO DEL SEEDING DE DATOS
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Obtenemos el contexto de base de datos
        var context = services.GetRequiredService<VotifyContext>();

        // Ejecutamos el inicializador
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        // En caso de que falle algo al insertar (muy útil para debugear)
        Console.WriteLine($"Ocurrió un error al poblar la base de datos: {ex.Message}");
    }
}
// FIN DEL SEEDING DE DATOS

app.Run();