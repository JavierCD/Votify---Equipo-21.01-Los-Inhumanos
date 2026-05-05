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

                    page.Background("#E8D093"); // fondo dorado suave

                    page.Content().Column(col =>
                    {
                        // TÍTULO
                        col.Item().AlignCenter().Text($"CERTIFICADO DE {posicion} PREMIO")
                            .FontSize(28).Bold();

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
    }
}
