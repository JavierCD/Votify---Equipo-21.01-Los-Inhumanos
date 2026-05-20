using Votify.Core.Models;

namespace Votify.Services.Interfaces
{
    public interface IAnalisisMejoraService
    {
        Task<HojaRutaMejora> GenerarHojaRutaAsync(int proyectoId);
    }
}
