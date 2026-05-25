using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Votify.Core.Models;
using Votify.Services.Interfaces;

namespace Votify.Services.Implementations
{
    public class CertificadoService : ICertificadoService
    {
        public byte[] GenerarCertificado(
    string nombreEquipo,
    List<string> integrantes,
    string posicion,
    string evento)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.Background("#FFFFFF"); // Fondo blanco limpio

                    page.Content().Column(col =>
                    {
                        // BORDE DECORATIVO (contenedor con borde)
                        col.Item().Border(2).BorderColor("#1E3A5F").Padding(20).Column(inner =>
                        {
                            // HEADER
                            inner.Item().AlignCenter().Text(text =>
                            {
                                text.Span("VOTIFY").FontSize(24).Bold().FontColor("#1E3A5F");
                                text.Span($" | CERTIFICADO DE {posicion} PREMIO").FontSize(18).FontColor("#6B7280");
                            });

                            inner.Item().PaddingTop(10);
                            inner.Item().LineHorizontal(2).LineColor("#1E3A5F");
                            inner.Item().PaddingTop(40);

                            // CUERPO
                            inner.Item().AlignCenter().Text("Se certifica que el equipo").FontSize(16).FontColor("#4B5563");

                            inner.Item().PaddingTop(20);
                            inner.Item().AlignCenter().Text(nombreEquipo)
                                .FontSize(28).Bold().FontColor("#1E3A5F");

                            inner.Item().PaddingTop(20);
                            inner.Item().AlignCenter().Text("Ha obtenido el")
                                .FontSize(16).FontColor("#4B5563");

                            inner.Item().PaddingTop(10);
                            inner.Item().AlignCenter().Text($"{posicion} PREMIO")
                                .FontSize(22).Bold().FontColor("#2563EB"); // Azul brillante para el premio

                            inner.Item().PaddingTop(20);
                            inner.Item().AlignCenter().Text("En el evento")
                                .FontSize(16).FontColor("#4B5563");

                            inner.Item().PaddingTop(10);
                            inner.Item().AlignCenter().Text(evento)
                                .FontSize(20).Italic().FontColor("#374151");

                            inner.Item().PaddingTop(30);
                            inner.Item().LineHorizontal(1).LineColor("#E5E7EB");
                            inner.Item().PaddingTop(20);

                            // INTEGRANTES
                            inner.Item().AlignCenter().Text("INTEGRANTES DEL EQUIPO").Bold().FontSize(14).FontColor("#6B7280");

                            inner.Item().PaddingTop(10);
                            foreach (var integrante in integrantes)
                            {
                                inner.Item().AlignCenter().Text($"• {integrante}").FontSize(14).FontColor("#4B5563");
                            }

                            // FIRMA DIGITAL
                            inner.Item().PaddingTop(40);
                            inner.Item().AlignCenter().Text("Firma digital verificada por Votify © 2026")
                                .FontSize(12).FontColor("#9CA3AF");
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }


        public byte[] GenerarCertificadoParticipacion(
    string nombreParticipante,
    string nombreProyecto,
    string nombreEvento,
    DateTime fechaParticipacion)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.Background("#FFFFFF");

                    page.Content().Column(col =>
                    {
                        // BORDE DECORATIVO (contenedor con borde)
                        col.Item().Border(2).BorderColor("#1E3A5F").Padding(20).Column(inner =>
                        {
                            // HEADER
                            inner.Item().AlignCenter().Text(text =>
                            {
                                text.Span("VOTIFY").FontSize(24).Bold().FontColor("#1E3A5F");
                                text.Span(" | CERTIFICADO DE PARTICIPACIÓN").FontSize(18).FontColor("#6B7280");
                            });

                            inner.Item().PaddingTop(10);
                            inner.Item().LineHorizontal(2).LineColor("#1E3A5F");
                            inner.Item().PaddingTop(40);

                            // CUERPO
                            inner.Item().AlignCenter().Text("Se certifica que").FontSize(16).FontColor("#4B5563");

                            inner.Item().PaddingTop(20);
                            inner.Item().AlignCenter().Text(nombreParticipante)
                                .FontSize(28).Bold().FontColor("#1E3A5F");

                            inner.Item().PaddingTop(20);
                            inner.Item().AlignCenter().Text("Ha participado activamente en el evento")
                                .FontSize(16).FontColor("#4B5563");

                            inner.Item().PaddingTop(10);
                            inner.Item().AlignCenter().Text(nombreEvento)
                                .FontSize(22).Bold().FontColor("#2563EB");

                            inner.Item().PaddingTop(20);
                            inner.Item().AlignCenter().Text("Con el proyecto")
                                .FontSize(16).FontColor("#4B5563");

                            inner.Item().PaddingTop(10);
                            inner.Item().AlignCenter().Text(nombreProyecto)
                                .FontSize(20).Italic().FontColor("#374151");

                            inner.Item().PaddingTop(40);
                            inner.Item().LineHorizontal(1).LineColor("#E5E7EB");
                            inner.Item().PaddingTop(20);

                            // FECHA
                            inner.Item().AlignCenter().Text($"Fecha de participación: {fechaParticipacion:dd/MM/yyyy}")
                                .FontSize(14).FontColor("#6B7280");

                            // FIRMA DIGITAL
                            inner.Item().PaddingTop(40);
                            inner.Item().AlignCenter().Text("Firma digital verificada por Votify © 2026")
                                .FontSize(12).FontColor("#9CA3AF");
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
