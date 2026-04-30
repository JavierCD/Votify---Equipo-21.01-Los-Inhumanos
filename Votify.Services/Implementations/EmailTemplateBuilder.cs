using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Services.Interfaces;
using Votify.Services.Models.DTOs;

namespace Votify.Services.Implementations
{
    public class EmailTemplateBuilder : IEmailTemplateBuilder
    {
        public string GenerarTablaResultadosHtml(string nombreCategoria, List<PosicionRankingDto> ranking)
        {
            var htmlBuilder = new StringBuilder();

            htmlBuilder.Append($@"
            <div style='font-family: Arial, sans-serif; color: #333; max-width: 600px; margin: 0 auto;'>
                <h2 style='color: #2c3e50; text-align: center;'>Resultados Oficiales</h2>
                <p>Hola, aquí tienes la clasificación final de la categoría <strong>'{nombreCategoria}'</strong> en la que participaste:</p>

                <table style='width: 100%; border-collapse: collapse; margin-top: 20px;'>
                    <thead style='background-color: #0d6efd; color: white;'>
                        <tr>
                            <th style='padding: 12px; border: 1px solid #ddd;'>Posición</th>
                            <th style='padding: 12px; border: 1px solid #ddd; text-align: left;'>Proyecto</th>
                            <th style='padding: 12px; border: 1px solid #ddd;'>Votos/Puntos</th>
                        </tr>
                    </thead>
                    <tbody>");

            foreach (var pos in ranking)
            {
                string colorFila = pos.Posicion % 2 == 0 ? "#f8f9fa" : "#ffffff";

                htmlBuilder.Append($@"
                        <tr style='background-color: {colorFila}; text-align: center;'>
                            <td style='padding: 10px; border: 1px solid #ddd; font-weight: bold;'>{pos.Posicion}º</td>
                            <td style='padding: 10px; border: 1px solid #ddd; text-align: left;'>{pos.Medalla}{pos.NombreProyecto}</td>
                            <td style='padding: 10px; border: 1px solid #ddd;'>
                                <span style='background-color: #e9ecef; padding: 4px 8px; border-radius: 12px; font-size: 0.9em;'>{pos.PuntosTotales} pts</span>
                            </td>
                        </tr>");
            }

            htmlBuilder.Append(@"
                    </tbody>
                </table>
                <p style='margin-top: 30px; font-size: 0.9em; color: #6c757d; text-align: center;'>
                    Gracias por participar en el evento.<br>El Equipo de Votify.
                </p>
            </div>");

            return htmlBuilder.ToString();
        }
    }
}
