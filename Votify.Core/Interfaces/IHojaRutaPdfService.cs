using Votify.Core.Models;

namespace Votify.Core.Interfaces
{
    public interface IHojaRutaPdfService
    {
        byte[] GenerarPdf(HojaRutaMejora hojaRuta);
    }
}
