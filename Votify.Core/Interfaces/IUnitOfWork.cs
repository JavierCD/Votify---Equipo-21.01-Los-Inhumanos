using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Models;

namespace Votify.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Evento> Eventos { get; }
        IGenericRepository<Proyecto> Proyectos { get; }
        IGenericRepository<Categoria> Categorias { get; }
        IGenericRepository<Voto> Votos { get; }
        IGenericRepository<Premio> Premios { get; }
        IGenericRepository<Criterio> Criterios { get; }
        IGenericRepository<DetalleVoto> DetallesVoto { get; }
        IGenericRepository<Miembro> Miembros { get; }
        IGenericRepository<Votacion> Votaciones { get; }
        IGenericRepository<Votante> Votantes { get; }
        IGenericRepository<Participante> Participantes { get; }
        IGenericRepository<Juez> Jueces { get; }
        IGenericRepository<Organizador> Organizadores { get; }
        IGenericRepository<Notificacion> Notificaciones { get; }

        IEventoRepository EventoRepository { get; }
        ICategoriaRepository CategoriaRepository { get; }
        IParticipanteRepository ParticipanteRepository { get; }
        IPopularRepository PopularRepository { get; }
        IPuntuacionRepository PuntuacionRepository { get; }
        IVotoExpertoRepository VotoExpertoRepository { get; }
        IVotoPopularRepository VotoPopularRepository { get; }
        IVotoPuntuacionRepository VotoPuntuacionRepository { get; }
        IVotoMulticriterioRepository VotoMulticriterioRepository { get; }
        IAuditoriaRepository AuditoriaRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
