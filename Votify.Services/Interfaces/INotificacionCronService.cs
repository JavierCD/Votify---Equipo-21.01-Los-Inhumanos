using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models;

namespace Votify.Services.Interfaces
{
    public interface INotificacionCronService
    {
        Task ProcesarAperturasDeVotacionAsync();
    }
}
