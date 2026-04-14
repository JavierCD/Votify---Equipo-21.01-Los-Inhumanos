using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models;
using Votify.Persistence.Context;
using Votify.Core.Interfaces;

namespace Votify.Persistence.Repositories
{
    public class ParticipanteRepository : GenericRepository<Participante>, IParticipanteRepository
    {
        // El constructor recibe el contexto y se lo pasa a la clase base
        public ParticipanteRepository(VotifyContext context) : base(context)
        {
        }

        // Aquí metemos la Mega-Consulta que ensucia, lejos de la capa de negocio
        public async Task<Participante?> ObtenerConDetallesDashboardAsync(int id)
        {
            return await _context.Set<Participante>()
                .Include(p => p.Proyectos)
                   .ThenInclude(proy => proy.Votos.Where(v => v.Comentario != null && EF.Property<string>(v, "TipoVoto") == "Experto"))
                .Include(p => p.Proyectos)
                    .ThenInclude(proy => proy.Categorias)
                        .ThenInclude(cat => cat.Evento)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Participante?> ObtenerPorIdAsync(int id)
        {
            return await _context.Set<Participante>()
                .Include(p => p.Proyectos)
                    .ThenInclude(proy => proy.Categorias) 
                        .ThenInclude(c => c.Votacion)     
                            .ThenInclude(v => v.Votos)    
                                .ThenInclude(v => v.Proyecto) 
                .FirstOrDefaultAsync(p => p.Id == id);
        }

    }
}
