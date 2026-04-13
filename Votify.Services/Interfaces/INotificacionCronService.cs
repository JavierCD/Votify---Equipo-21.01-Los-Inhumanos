using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Interfaces
{
    public interface INotificacionCronService
    {
        Task ProcesarAperturasDeVotacionAsync();
    }
}
