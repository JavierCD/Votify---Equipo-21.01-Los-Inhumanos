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
                    page.Margin(30);
                    page.Background("#F8F4E8"); // fondo dorado suave

                    page.Content().Column(col =>
                    {
                        // TÍTULO
                        col.Item().AlignCenter().Text(text =>
                        {
                            text.Span($"CERTIFICADO DE {posicion} PREMIO")
                                .FontSize(30)
                                .Bold()
                                .FontColor("#1E3A5F");
                        });

                        col.Item().LineHorizontal(1);

                        col.Item().PaddingTop(20);

                        // FELICIDADES
                        col.Item().AlignCenter().Text($"FELICIDADES {nombreEquipo}")
                            .FontSize(20).Bold();

                        col.Item().PaddingTop(20);

                        // EVENTO
                        col.Item().AlignCenter().Text($"Evento: {evento}")
                            .FontSize(16);

                        col.Item().PaddingTop(20);

                        // PREMIO
                        col.Item().AlignCenter().Text("VUESTRO PREMIO ES")
                            .FontSize(16).Bold();

                        col.Item().AlignCenter().Text(posicion)
                            .FontSize(22).Bold().FontColor("#666666");

                        col.Item().PaddingTop(20);
                        col.Item().LineHorizontal(1);

                        col.Item().PaddingTop(30);

                        // BLOQUE FINAL
                        col.Item().Row(row =>
                        {
                            // IZQUIERDA → INTEGRANTES
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("INTEGRANTES DEL EQUIPO")
                                    .Bold();

                                foreach (var integrante in integrantes)
                                {
                                    c.Item().Text($"• {integrante}");
                                }
                            });

                            // DERECHA → MEDALLA (simple)
                            row.ConstantItem(120).Height(120).Background("#C9A24A")
                                .AlignCenter().AlignMiddle()
                                .Text("🏅").FontSize(40);
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
