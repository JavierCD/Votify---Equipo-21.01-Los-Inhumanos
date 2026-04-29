using Microsoft.EntityFrameworkCore;
using Radzen;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Persistence.Context;
using Votify.Persistence.Repositories;
using Votify.Services.Implementations;
using Votify.Services.Interfaces;
using Votify.UI;
using Votify.Web.Components;
using Votify.Web.Services;

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
builder.Services.AddSingleton<UserSession>();

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
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
//builder.Services.AddScoped<IVotanteRepository, VotanteRepository>();

// builder.Services.AddScoped<IVotanteService, VotanteService>(); // Descomenta cuando lo necesites
// 2. Registramos el servicio de Eventos que acabamos de crear
builder.Services.AddScoped<IProyectoService, ProyectoService>();
builder.Services.AddScoped<UserSession>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPopularService, PopularService>();
builder.Services.AddScoped<IPopularRepository, PopularRepository>();
builder.Services.AddScoped<IVotoPopularRepository, VotoPopularRepository>();
builder.Services.AddScoped<IVotoPopularService, VotoPopularService>();
builder.Services.AddScoped<IPuntuacionService, PuntuacionService>();
builder.Services.AddScoped<IPuntuacionRepository, PuntuacionRepository>();
builder.Services.AddScoped<IVotoPuntuacionService, VotoPuntuacionService>();
builder.Services.AddScoped<IVotoPuntuacionRepository, VotoPuntuacionRepository>();
builder.Services.AddScoped<IVotoExpertoRepository, VotoExpertoRepository>();
builder.Services.AddScoped<IVotoExpertoServices, VotoExpertoService>();
builder.Services.AddScoped<IEventoRepository, EventoRepository>();
builder.Services.AddScoped<IEventoService, EventoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IGenericRepository<Miembro>, GenericRepository<Miembro>>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IGenericRepository<Votante>, GenericRepository<Votante>>();
builder.Services.AddScoped<IPlantillaBaremoService, PlantillaBaremoService>();
builder.Services.AddScoped<IMulticriterioService, MulticriterioService>();
// builder.Services.AddScoped<IVotanteService, VotanteService>(); // Descomenta cuando lo necesites
builder.Services.AddScoped<IGenericRepository<Juez>, GenericRepository<Juez>>();

// 3. Registramos el servicio de Participantes 
builder.Services.AddScoped<IParticipanteService, ParticipanteService>();
builder.Services.AddScoped<IParticipanteRepository, ParticipanteRepository>();
// 4. Registramos el servicio que contiene la lógica
builder.Services.AddScoped<INotificacionCronService, NotificacionCronService>();
// 5. Registramos el Vigilante para que .NET lo arranque en segundo plano
builder.Services.AddHostedService<NoctificacionBackgroundService>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();

builder.Services.AddScoped<NotificationService>();

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