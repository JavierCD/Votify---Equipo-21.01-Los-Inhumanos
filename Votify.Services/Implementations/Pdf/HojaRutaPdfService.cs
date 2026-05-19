using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Votify.Core.Interfaces;
using Votify.Core.Models;

namespace Votify.Services.Implementations.Pdf
{
    public class HojaRutaPdfService : IHojaRutaPdfService
    {
        public byte[] GenerarPdf(HojaRutaMejora hojaRuta)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.PageColor(Colors.White);

                    page.Header().Column(col =>
                    {
                        col.Item().Text("VOTIFY - HOJA DE RUTA DE MEJORA")
                            .FontSize(24).Bold().FontColor("#1E3A5F");

                        col.Item().PaddingTop(4).LineHorizontal(2).LineColor("#1E3A5F");

                        col.Item().PaddingTop(8).Row(row =>
                        {
                            row.RelativeItem().Text($"Proyecto: {hojaRuta.ProyectoNombre}")
                                .FontSize(14).FontColor("#444444");

                            row.RelativeItem().AlignRight().Text($"Generado: {hojaRuta.FechaGeneracion:dd/MM/yyyy HH:mm}")
                                .FontSize(12).FontColor("#888888");
                        });

                        col.Item().PaddingTop(4).Row(row =>
                        {
                            row.RelativeItem().Text($"Comentarios analizados: {hojaRuta.TotalComentariosAnalizados}")
                                .FontSize(12).FontColor("#666666");

                            row.RelativeItem().AlignRight().Text($"Sugerencias: {hojaRuta.Sugerencias.Count}")
                                .FontSize(12).FontColor("#666666");
                        });
                    });

                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        if (!hojaRuta.Sugerencias.Any())
                        {
                            col.Item().PaddingTop(40).AlignCenter().Text("No se generaron sugerencias.")
                                .FontSize(16).FontColor("#999999");
                            return;
                        }

                        foreach (var sug in hojaRuta.Sugerencias)
                        {
                            var colorBorde = sug.Prioridad switch
                            {
                                1 => Colors.Red.Medium,
                                2 => Colors.Orange.Medium,
                                3 => Colors.Blue.Medium,
                                _ => Colors.Grey.Medium
                            };

                            var etiquetaPrioridad = sug.Prioridad switch
                            {
                                1 => "URGENTE",
                                2 => "IMPORTANTE",
                                3 => "RECOMENDADO",
                                _ => "SUGERIDO"
                            };

                            col.Item().PaddingBottom(16).BorderLeft(4).BorderColor(colorBorde).PaddingLeft(12).Column(sugCol =>
                            {
                                sugCol.Item().Row(row =>
                                {
                                    row.AutoItem().Background(colorBorde).PaddingHorizontal(8).PaddingVertical(2)
                                        .Text(etiquetaPrioridad).FontSize(10).Bold().FontColor(Colors.White);

                                    row.AutoItem().PaddingLeft(8).Text(sug.Categoria)
                                        .FontSize(11).FontColor("#666666");
                                });

                                sugCol.Item().PaddingTop(6).Text(sug.Descripcion)
                                    .FontSize(15).Bold().FontColor("#1E3A5F");

                                sugCol.Item().PaddingTop(4).Row(row =>
                                {
                                    row.AutoItem().Text("Accion: ").FontSize(12).Bold().FontColor("#444444");
                                    row.RelativeItem().Text(sug.AccionRecomendada).FontSize(12).FontColor("#444444");
                                });
                            });
                        }
                    });

                    page.Footer().AlignBottom().Column(col =>
                    {
                        col.Item().LineHorizontal(1).LineColor("#CCCCCC");
                        col.Item().PaddingTop(6).AlignCenter().Text("Generado automaticamente por Votify IA")
                            .FontSize(10).FontColor("#AAAAAA");
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
