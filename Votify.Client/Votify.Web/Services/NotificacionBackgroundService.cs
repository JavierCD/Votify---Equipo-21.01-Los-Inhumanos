using Votify.Services.Interfaces;

namespace Votify.Web.Services
{
    public class NoctificacionBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public NoctificacionBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Este bucle se ejecutará infinitamente mientras el servidor esté encendido
            while (!stoppingToken.IsCancellationRequested)
            {
                // TRUCO DE ARQUITECTURA: Los BackgroundService son "Singletons" (viven siempre).
                // Pero nuestra base de datos es "Scoped" (vive por petición).
                // Tenemos que crear un "Scope" artificial para pedir el servicio:
                using (var scope = _serviceProvider.CreateScope())
                {
                    var cronService = scope.ServiceProvider.GetRequiredService<INotificacionCronService>();

                    try
                    {
                        await cronService.ProcesarAperturasDeVotacionAsync();
                        await cronService.ProcesarRecordatoriosCierreAsync();
                        await cronService.ProcesarCierresDeVotacionAsync();
                    }
                    catch (Exception ex)
                    {
                        // Aquí en el futuro puedes meter un ILogger para registrar errores
                        Console.WriteLine($"Error en el Vigilante: {ex.Message}");
                    }
                }

                // El vigilante se echa a dormir 1 minuto antes de volver a mirar el reloj
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}

