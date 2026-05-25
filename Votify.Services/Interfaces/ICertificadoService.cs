using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Interfaces
{
    public interface ICertificadoService
    {
        byte[] GenerarCertificado(
            string nombreEquipo,
            List<string> integrantes,
            string posicion,
            string evento);

        byte[] GenerarCertificadoParticipacion(
            string nombreParticipante,
            string nombreProyecto,
            string nombreEvento,
            DateTime fechaParticipacion);
    }
}
