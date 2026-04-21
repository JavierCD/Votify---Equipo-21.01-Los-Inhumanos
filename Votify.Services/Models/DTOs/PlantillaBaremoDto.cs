using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Services.Models.DTOs
{
    public record CriterioPlantillaDto(string Nombre, int Peso);

    public record PlantillaBaremoDto(
    string Id,
    string Titulo,
    string Descripcion,
    List<CriterioPlantillaDto> Criterios
    );
}
