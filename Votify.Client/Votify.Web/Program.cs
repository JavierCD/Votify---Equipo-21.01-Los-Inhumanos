using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Radzen;
using Votify.Core.Interfaces;
using Votify.Core.Models;
using Votify.Persistence.Context;
using Votify.Persistence.UnitOfWork;
using Votify.Services.Implementations;
using Votify.Services.Implementations.Analysis;
using Votify.Services.Implementations.IA;
using Votify.Services.Implementations.Pdf;
using Votify.Services.Implementations.Observers;
using Votify.Services.Interfaces;
using Votify.UI;
using Votify.Web.Components;
using Votify.Web.Services;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

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
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IProyectoService, ProyectoService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPopularService, PopularService>();
builder.Services.AddScoped<IVotoPopularService, VotoPopularService>();
builder.Services.AddScoped<IPuntuacionService, PuntuacionService>();
builder.Services.AddScoped<IVotoPuntuacionService, VotoPuntuacionService>();
builder.Services.AddScoped<IVotoExpertoServices, VotoExpertoService>();
builder.Services.AddScoped<IEventoService, EventoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPlantillaBaremoService, PlantillaBaremoService>();
builder.Services.AddScoped<IMulticriterioService, MulticriterioService>();
builder.Services.AddScoped<IVotoMulticriterioService, VotoMulticriterioService>();
builder.Services.AddScoped<IResultadosService, ResultadosService>();
builder.Services.AddScoped<IEmailTemplateBuilder, EmailTemplateBuilder>();
builder.Services.AddScoped<IVotacionService, VotacionService>();
builder.Services.AddScoped<IParticipanteService, ParticipanteService>();
builder.Services.AddScoped<ISupervisionService, SupervisionService>();
builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();
builder.Services.AddScoped<ICertificadoService, CertificadoService>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();

builder.Services.AddHostedService<NotificacionBackgroundService>();

builder.Services.AddScoped<NotificationService>();

// Observer Pattern - Votacion State Notifications
builder.Services.AddSingleton<IVotacionStateSubject, VotacionStateSubject>();
builder.Services.AddScoped<IVotacionStateObserver, AperturaNotificationObserver>();
builder.Services.AddScoped<IVotacionStateObserver, CierreNotificationObserver>();
builder.Services.AddScoped<IVotacionStateObserver, RecordatorioObserver>();
builder.Services.AddSingleton<IVotacionStateObserver, RealTimeUINotificationObserver>();
builder.Services.AddScoped<VotacionStateCronDetector>();
builder.Services.AddSingleton<RealTimeUINotificationObserver>();

// IA - Análisis de Mejora
builder.Services.AddHttpClient<IIAProvider, OllamaProvider>(client =>
{
    client.BaseAddress = new Uri("http://localhost:11434");
    client.Timeout = TimeSpan.FromMinutes(2);
});
builder.Services.AddScoped<IAnalisisMejoraService, AnalisisMejoraService>();
builder.Services.AddScoped<IHojaRutaPdfService, HojaRutaPdfService>();
builder.Services.AddScoped<ICriteriosSugeridosService, CriteriosSugeridosService>();



var app = builder.Build();
app.MapGet("/certificado", (
    string nombreEquipo,
    string integrantes,
    string posicion,
    string evento,
    ICertificadoService certificadoService) =>
{
    var listaIntegrantes = integrantes.Split(';').ToList();

    var pdf = certificadoService.GenerarCertificado(
        nombreEquipo,
        listaIntegrantes,
        posicion,
        evento
    );

    return Results.File(pdf, "application/pdf");
});

app.MapGet("/hoja-ruta-pdf", async (
    int proyectoId,
    IAnalisisMejoraService analisisService,
    IHojaRutaPdfService pdfService) =>
{
    var hojaRuta = await analisisService.GenerarHojaRutaAsync(proyectoId);
    var pdf = pdfService.GenerarPdf(hojaRuta);
    return Results.File(pdf, "application/pdf", $"HojaRuta_{proyectoId}.pdf");
});

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